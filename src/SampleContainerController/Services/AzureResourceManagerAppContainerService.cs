using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppContainers;
using Azure.ResourceManager.AppContainers.Models;
using Azure.ResourceManager.Resources;
using SampleContainerController.Dto;
using SampleContainerController.Interface;


namespace SampleContainerController.Services
{
    public class AzureResourceManagerAppContainerService : IAppContainerService
    {
        public async Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            if(job == null) throw new ArgumentNullException(nameof(job));

            var resourceGroupName = "development"; // TODO:
            var jobName = job.Name;

            var clientOptions = new ArmClientOptions();

            /*
                Force the client to use a working API version for Container Apps / Jobs 
                TODO: remove later when all Azure support latest jobs.
            */
            clientOptions.SetApiVersion(new Azure.Core.ResourceType("Microsoft.App/jobs"), "2026-01-01");

            var client = new ArmClient(new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned), defaultSubscriptionId: null, clientOptions);

            // Find default subscription.
            SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync(cancellationToken);
            var subscriptionId = subscription.Data.SubscriptionId;

            // Get a reference to existing Container App Job.
            ResourceIdentifier jobId = ContainerAppJobResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, jobName);
            ContainerAppJobResource acaJobResource = client.GetContainerAppJobResource(jobId);

            // Fetch the full details of the existing job to inspect its template and find actual container.
            var jobData = await acaJobResource.GetAsync(cancellationToken);
            var jobContainer = jobData.Value.Data.Template.Containers.FirstOrDefault(c => string.Equals(c.Name, jobName, StringComparison.OrdinalIgnoreCase));

            if (jobContainer == null)
            {
                throw new InvalidOperationException($"Could not find a container named '{jobName}' in the existing job definition.");
            }

            Console.WriteLine("Adding container job arguments, environment variables and commands.");

            var containerOverride = new JobExecutionContainer
            {
                Name = jobName,
                Image = jobContainer.Image  // Reuse current job image.
            };

            // Add args
            foreach(var arg in job.Args)
            {
                containerOverride.Args.Add(arg);
            }

            // Envs
            foreach(var env in job.Envs)
            {
                containerOverride.Env.Add(new ContainerAppEnvironmentVariable
                {
                    Name = env.Name,
                    Value = env.Value,
                    SecretRef = env.SecretRef
                });
            }

            // Commands
            foreach(var cmd in  job.Commands)
            {
                containerOverride.Command.Add(cmd);
            }

            // Create an execution template override to inject the new environment variables and arguments.
            var containerAppJobExecutionTemplate = new ContainerAppJobExecutionTemplate();
            containerAppJobExecutionTemplate.Containers.Add(containerOverride);

            Console.WriteLine($"Begin start [{jobName}].");

            // Start the job
            var operation = await acaJobResource.StartAsync(WaitUntil.Started, containerAppJobExecutionTemplate, cancellationToken);
            var rawResponse = operation.GetRawResponse();

            if (rawResponse.Status == 202 || rawResponse.Status == 200)
            {
                Console.WriteLine($"Successfully requested Azure to start the job [{jobName}] (HTTP {rawResponse.Status}).");
            }
        }
    }
}