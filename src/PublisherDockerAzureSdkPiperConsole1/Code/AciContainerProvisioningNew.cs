//using Azure;
//using Azure.Core;
//using Azure.Identity;
//using Azure.Provisioning;
//using Azure.Provisioning.ContainerInstance;
//using Azure.ResourceManager;


//namespace PublisherDockerAzureSdkPiperConsole1.Code
//{
//    /// <summary>
//    /// https://github.com/Azure/azure-sdk-for-net/tree/Azure.Provisioning.ContainerInstance_1.0.0-beta.1
//    /// </summary>
//    public class AciContainerProvisioningNew
//    {
//        public async Task LaunchReceiverContainerWithSdkAsync(WebhookPayload dto)
//        {
//            Infrastructure infra = new();

//            ProvisioningParameter image =
//                new(nameof(image), typeof(string))
//                {
//                    Value = "mcr.microsoft.com/azuredocs/aci-helloworld",
//                    Description = "Container image to deploy.",
//                };
//            infra.Add(image);

//            ProvisioningParameter port =
//                new(nameof(port), typeof(int))
//                {
//                    Value = 80,
//                    Description = "Port to open on the container and the public IP address.",
//                };
//            infra.Add(port);

//            ContainerGroup containerGroup =
//                new(nameof(containerGroup), ContainerGroup.ResourceVersions.V2025_09_01)
//                {
//                    ContainerGroupOSType = ContainerInstanceOperatingSystemType.Linux,
//                    RestartPolicy = ContainerGroupRestartPolicy.Always,
//                    IPAddress = new ContainerGroupIPAddress
//                    {
//                        AddressType = ContainerGroupIPAddressType.Public,
//                        Ports =
//                        {
//                new ContainerGroupPort { Port = port, Protocol = ContainerGroupNetworkProtocol.Tcp }
//                        },
//                    },
//                    Containers =
//                    {
//            new ContainerInstanceContainer
//            {
//                Name = "helloworld",
//                Image = image,
//                Ports =
//                {
//                    new ContainerPort { Port = port, Protocol = ContainerNetworkProtocol.Tcp }
//                },
//                Resources = new ContainerResourceRequirements
//                {
//                    Requests = new ContainerResourceRequestsContent
//                    {
//                        Cpu = 1,
//                        MemoryInGB = 1,
//                    },
//                },
//            }
//                    },
//                };
//            infra.Add(containerGroup);

//            infra.Add(new ProvisioningOutput("containerIPv4Address", typeof(string)) { Value = containerGroup.IPAddress.IP });

//            var provisioningPlan = infra.Build();
//        }

//        public async Task Start(string resourceGroup, ProvisioningPlan plan)
//        {
//            var credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
//            var armClient = new ArmClient(credential);

//            Console.WriteLine("Deploying container group to Azure...");

//            //   var options = new ProvisioningDeploymentOptions();

//#pragma warning disable AZPROVISION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//            var options = new ProvisioningDeploymentOptions
//            {
//                ArmClient = armClient,
//            };

//            var deploymentResult =  await plan.DeployToResourceGroupAsync(resourceGroup, options, default);

//            var outputs = deploymentResult.Deployment.Data.Properties.Outputs;

//#pragma warning restore AZPROVISION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//            if (outputs != null && outputs.ToString().Contains("containerIPv4Address"))
//            {
//                Console.WriteLine("Container deployed successfully!");
//                Console.WriteLine($"Outputs JSON: {outputs}");
//            }
//        }
//    }
//}
