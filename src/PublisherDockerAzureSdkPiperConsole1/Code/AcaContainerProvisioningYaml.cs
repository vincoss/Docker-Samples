using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppContainers;
using Azure.ResourceManager.AppContainers.Models;
using Azure.ResourceManager.Resources;
using System.ClientModel.Primitives;
using System.Text.Json;
using YamlDotNet.Serialization;



namespace PublisherDockerAzureSdkPiperConsole1
{
    public class AcaContainerProvisioningYaml
    {
        public async Task LaunchReceiverContainerWithSdkAsyncCreate(WebhookPayload dto)
        {
            string yamlPath = "job-config.yml";
            if (!File.Exists(yamlPath))
            {
                Console.WriteLine($"Error: {yamlPath} not found.");
                return;
            }

            Console.WriteLine("Reading YML Configuration...");
            string yamlText = await File.ReadAllTextAsync(yamlPath);

            // 1. Parse YAML to a generic object graph
            var yamlDeserializer = new DeserializerBuilder().Build();
            var yamlGraph = yamlDeserializer.Deserialize<dynamic>(yamlText);

            // 2. Transcode specific segments to JSON strings for Azure SDK parsing
            string manualTriggerJson = JsonSerializer.Serialize(yamlGraph["manualTriggerConfig"]);

            // Extract top level properties safely from our dynamic object
            string jobName = yamlGraph["jobName"];
            string resourceGroupName = yamlGraph["resourceGroupName"];
            string environmentId = yamlGraph["environmentId"];
            string locationName = yamlGraph["location"];
            string containerImage = yamlGraph["image"];
            double cpu = double.Parse(yamlGraph["cpu"]);
            string memory = yamlGraph["memory"];

            // 3. Deserialize natively using Azure SDK's ModelReaderWriter
            JobConfigurationManualTriggerConfig triggerConfig = ModelReaderWriter.Read<JobConfigurationManualTriggerConfig>(
                BinaryData.FromString(manualTriggerJson)
            );

            Console.WriteLine($"Parsed trigger configurations: Parallelism={triggerConfig.Parallelism}, Completions={triggerConfig.ReplicaCompletionCount}");

            // 4. Connect to Azure Resource Manager (ARM)
            Console.WriteLine("Authenticating with Azure...");
            ArmClient client = new ArmClient(new DefaultAzureCredential());

            // Extract Subscription ID out of the environment resource ID path
            var envResourceId = new ResourceIdentifier(environmentId);
            SubscriptionResource subscription = client.GetSubscriptionResource(
                ResourceIdentifier.Parse($"/subscriptions/{envResourceId.SubscriptionId}")
            );
            ResourceGroupResource resourceGroup = await subscription.GetResourceGroups().GetAsync(resourceGroupName);



            // 5. Build up the complete ContainerAppJobData Definition Object
            var jobData = new ContainerAppJobData(new AzureLocation(locationName))
            {
                EnvironmentId = envResourceId,
                Configuration = new ContainerAppJobConfiguration(ContainerAppJobTriggerType.Manual, 1800)
                {
                    ReplicaRetryLimit = 1,
                    ManualTriggerConfig = triggerConfig // Embedded parsed YAML model
                },
                Template = new ContainerAppJobTemplate()
                {
                    Containers =
                    {
                        new ContainerAppContainer()
                        {
                            Name = "job-processor",
                            Image = containerImage,
                            Resources = new AppContainerResources()
                            {
                                Cpu = cpu,
                                Memory = memory
                            }
                        }
                    }
                }
            };

            // 6. Create or Update the Container App Job
            Console.WriteLine($"Creating Container App Job '{jobName}' in Azure...");
            ContainerAppJobCollection jobCollection = resourceGroup.GetContainerAppJobs();

            ArmOperation<ContainerAppJobResource> createOperation = await jobCollection.CreateOrUpdateAsync(
                WaitUntil.Completed,
                jobName,
                jobData
            );
            ContainerAppJobResource createdJob = createOperation.Value;
            Console.WriteLine($"Job successfully verified/created. Resource ID: {createdJob.Id}");

            // 7. Fire an instance of the job on-demand
            Console.WriteLine("Triggering manual job execution...");

            // Set up optional trigger configurations for this run instance if desired
            var executionTrigger = new ContainerAppJobExecutionTemplate();

            ArmOperation<ContainerAppJobExecutionBase> startOperation = await createdJob.StartAsync(
                WaitUntil.Completed,
                executionTrigger
            );

            Console.WriteLine($"Job Execution triggered successfully! Execution ID Name: {startOperation.Value.Name}");

        }

    }
}

