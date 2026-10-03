using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppContainers;
using Azure.ResourceManager.AppContainers.Models;
using SampleAzureContainerController.Dto;
using SampleAzureContainerController.Interface;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SampleAzureContainerController.Services
{
    public class AppContainerService : IAppContainerService
    {
        private readonly ArmClient? _armClient;
        private readonly string _subscriptionId;
        private readonly string _resourceGroupName;
        private readonly ILogger<AppContainerService> _logger;

        public AppContainerService(string resourceGroupName, ILogger<AppContainerService> logger)
        {
            _resourceGroupName = resourceGroupName ?? throw new ArgumentNullException(nameof(resourceGroupName));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            /*
                Force the client to use a working API version for Container Apps / Jobs 
                TODO: remove later when all Azure support latest jobs.
            */
            var clientOptions = new ArmClientOptions();
            clientOptions.SetApiVersion(new Azure.Core.ResourceType("Microsoft.App/jobs"), "2026-01-01");

            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = null // Automatically falls back to SystemAssigned or local context
            });

            _logger.LogDebug("Begin fetch default subscription");

            _armClient = new ArmClient(new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned), defaultSubscriptionId: null, clientOptions);

            // Requires Reader role.
            var subscription = _armClient.GetDefaultSubscription();
            _subscriptionId = subscription.Data.SubscriptionId;
        }

        public async Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            _logger.LogDebug(nameof(StartAsync));
            _logger.LogInformation("Fetching job resource identifier for container job: [{JobName}]", job.Name);

            // Get a reference to existing Container App Job.
            var jobId = ContainerAppJobResource.CreateResourceIdentifier(_subscriptionId, _resourceGroupName, job.Name);
            var acaJobResource = _armClient.GetContainerAppJobResource(jobId);

            // Fetch the full details of the existing job to inspect its template and find actual container.
            var jobData = await acaJobResource.GetAsync(cancellationToken).ConfigureAwait(false);
            var jobContainer = jobData.Value.Data.Template.Containers.FirstOrDefault(c => string.Equals(c.Name, job.Name, StringComparison.OrdinalIgnoreCase));

            if (jobContainer == null)
            {
                throw new InvalidOperationException($"Target container execution failed. Could not locate a container template named '{job.Name}' within Azure Resource Group '{_resourceGroupName}'.");
            }

            _logger.LogInformation("Compiling job configuration overrides for container job: [{JobName}]", job.Name);

            // Add execution parameter overrides
            var containerOverride = new JobExecutionContainer
            {
                Name = job.Name,
                Image = !string.IsNullOrWhiteSpace(job.ImageName) ? job.ImageName : jobContainer.Image
            };

            // Add args
            foreach (var arg in job.Args)
            {
                containerOverride.Args.Add(arg);
            }

            // Envs
            foreach (var env in job.Envs)
            {
                containerOverride.Env.Add(new ContainerAppEnvironmentVariable
                {
                    Name = env.Name,
                    Value = env.Value,
                    SecretRef = env.SecretRef
                });
            }

            // Commands
            foreach (var cmd in job.Commands)
            {
                containerOverride.Command.Add(cmd);
            }

            // Create an execution template override to inject the new environment variables and arguments.
            var executionTemplate = new ContainerAppJobExecutionTemplate();
            executionTemplate.Containers.Add(containerOverride);

            _logger.LogInformation("Dispatching trigger request to Azure for job: [{JobName}]", job.Name);

            // Dispatch the fire-and-forget/start directive to Azure Resource Manager.
            var operation = await acaJobResource.StartAsync(WaitUntil.Started, executionTemplate, cancellationToken).ConfigureAwait(false);
            var rawResponse = operation.GetRawResponse();

            if (rawResponse.Status == 202 || rawResponse.Status == 200)
            {
                _logger.LogInformation("Successfully requested Azure to start job [{JobName}] (HTTP {Status}).", job.Name, rawResponse.Status);
            }
        }
    }
}