using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppContainers;
using Azure.ResourceManager.AppContainers.Models;
using Azure.ResourceManager.Resources;



namespace PublisherDockerAzureSdkPiperConsole1
{
    public class AcaContainerProvisioning
    {
        public async Task LaunchReceiverContainerWithSdkAsyncCreate(WebhookPayload dto)
        {
            // 1. Initialise the ArmClient
            ArmClientOptions clientOptions = new ArmClientOptions();

            // Force the client to use a working API version for Container Apps / Jobs
            clientOptions.SetApiVersion(new Azure.Core.ResourceType("Microsoft.App/jobs"), "2026-01-01");

            // Initialize ArmClient using the custom options
            ArmClient client = new ArmClient(new DefaultAzureCredential(), defaultSubscriptionId: null, clientOptions);

            string subscriptionId = "";
            string resourceGroupName = "development";
            string environmentName = "WebApi-env-20260903153212"; // Must already exist
            string jobName = "test1";

            // 2. Fetch the Resource Group reference
            ResourceIdentifier resourceGroupResourceId = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
            ResourceGroupResource resourceGroup = client.GetResourceGroupResource(resourceGroupResourceId);

            // 3. Access the Job Collection for this Resource Group
            ContainerAppJobCollection jobCollection = resourceGroup.GetContainerAppJobs();

            // 4. Construct the baseline configuration for the Job
            // Gather the Resource ID of your Container Apps Environment
            string environmentId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.App/managedEnvironments/{environmentName}";

            // Pass BOTH the Trigger Type and Replica Timeout (e.g., 1800 seconds) into the constructor
            var manualConfig = new JobConfigurationManualTriggerConfig
            {
                Parallelism = 50,
                ReplicaCompletionCount = 50
            };

            var jobConfig = new ContainerAppJobConfiguration(ContainerAppJobTriggerType.Manual, 1800) // Maximum limit: 18,000 seconds.
            {
                ReplicaRetryLimit = 1, // 0 vs 1: Setting the limit to 0 means the job fails immediately if it crashes, without any retries.
                ManualTriggerConfig = manualConfig
            };

            var jobData = new ContainerAppJobData(AzureLocation.EastUS)
            {
                EnvironmentId = environmentId,
                Configuration = jobConfig,
                Template = new ContainerAppJobTemplate()
                {
                    Containers =
                {
                    // Fix: Use the generic ContainerAppContainer class here
                    new ContainerAppContainer()
                    {
                    Name = "my-primary-job-container",
                    Image = "mcr.microsoft.com/k8se/quickstart-jobs:latest",
                    //Image = "mcr.microsoft.com/azuredocs/aci-helloworld:latest",
                    Resources = new AppContainerResources
                    {
                        Cpu = 0.5,
                        Memory = "1.0Gi"
                                }
                            }
                        }
                }
            };

            Console.WriteLine($"Checking if Job '{jobName}' exists or needs provisioning...");

            // 5. Create or Update the job infrastructure 
            ArmOperation<ContainerAppJobResource> createJobOperation = await jobCollection.CreateOrUpdateAsync(WaitUntil.Completed, jobName, jobData);
            ContainerAppJobResource acaJob = createJobOperation.Value;

            Console.WriteLine($"Job infrastructure verified/created successfully.");

            // 6. Build the dynamic runtime override parameters for this specific execution
            string dynamicConnectionString = $"Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd={Guid.NewGuid()};";

            var executionTemplate = new ContainerAppJobExecutionTemplate();
            var containerOverride = new JobExecutionContainer
            {
                Name = "my-primary-job-container", // Must align with the template name given above
                Image = "mcr.microsoft.com/k8se/quickstart-jobs:latest",
            };

            containerOverride.Env.Add(new ContainerAppEnvironmentVariable
            {
                Name = "CONNECTION_STRING",
                Value = dynamicConnectionString
            });

            executionTemplate.Containers.Add(containerOverride);

            Console.WriteLine("Invoking job execution with dynamic connection string...");

            // 7. Fire off the job runtime execution with overrides
            ArmOperation<ContainerAppJobExecutionBase> jobExecutionOperation = await acaJob.StartAsync(WaitUntil.Completed, executionTemplate);
            ContainerAppJobExecutionBase executionResult = jobExecutionOperation.Value;

            Console.WriteLine($"Execution Status: {executionResult.Id}");
            Console.WriteLine($"Execution ID/Name: {executionResult.Name}");
        }

       public async Task LaunchReceiverContainerWithSdkAsyncExisting(WebhookPayload dto)
        {
            // 1. Authenticate with Azure (uses local CLI login, Environment variables, or Managed Identity)
            ArmClient client = new ArmClient(new DefaultAzureCredential());

            // 2. Define your Azure Resource IDs
            string subscriptionId = "YOUR_SUBSCRIPTION_ID";
            string resourceGroupName = "myResourceGroup";
            string jobName = "my-aca-cron-job";

            // 3. Get a reference to your existing Container App Job
            ResourceIdentifier jobId = ContainerAppJobResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, jobName);
            ContainerAppJobResource acaJob = client.GetContainerAppJobResource(jobId);

            // 4. Generate your dynamic connection string / variable for this specific run
            string dynamicConnectionString = $"Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd={Guid.NewGuid()};";

            // 5. Create an execution template override to inject the new environment variable
            var executionTemplate = new ContainerAppJobExecutionTemplate();

            var containerOverride = new JobExecutionContainer
            {
                Name = "my-primary-job-container" // Must match the container name defined in the Job
            };

            // Add the dynamic environment variable
            containerOverride.Env.Add(new ContainerAppEnvironmentVariable
            {
                Name = "CONNECTION_STRING",
                Value = dynamicConnectionString
            });

            executionTemplate.Containers.Add(containerOverride);

            Console.WriteLine($"Triggering ACA Job execution with unique connection string...");

            // 6. Start the job execution and pass the template overrides
            // This spins up the container, injects the variable, executes it, and shuts it down.
            ArmOperation<ContainerAppJobExecutionBase> operation = await acaJob.StartAsync(WaitUntil.Completed, executionTemplate);

            ContainerAppJobExecutionBase executionResult = operation.Value;

            Console.WriteLine($"Job Execution Triggered Successfully!");
            Console.WriteLine($"Execution Name: {executionResult.Name}");
            Console.WriteLine($"Execution Status: {executionResult.Id}"); // Typically "Running" or "Succeeded"
        }

    }
}

