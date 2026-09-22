using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Azure.ResourceManager.ContainerInstance.Models;
using Azure.ResourceManager.ManagedServiceIdentities;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;



namespace PublisherDockerAzureSdkPiperConsole1
{
    /// <summary>
    /// NO stdin at a moment
    /// </summary>
    public class AciContainerProvisioning
    {
       public async Task LaunchReceiverContainerWithSdkAsyncWorksOnPublicRepoNotArc(WebhookPayload dto)
        {
            // 1. Initialize client using local 'az login' credentials

            //var managedIdentityId = ManagedIdentityId.FromUserAssignedObjectId("bc9df52a-c2b4-41f8-96a2-69054af16a6d");
            //var credentialOptions = new ManagedIdentityCredentialOptions(managedIdentityId)
            //{

            //};
            //var credential = new ManagedIdentityCredential(credentialOptions);
            //new DefaultAzureCredential(); // Only to run code locally.

            var credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
            var armClient = new ArmClient(credential);

            try
            {
                Console.WriteLine($"{nameof(Environment.UserDomainName)}:   {Environment.UserDomainName}");
                Console.WriteLine($"{nameof(Environment.UserName)}:         {Environment.UserName}");

                // 2. Define target Azure resource coordinates
                string subscriptionId = "{subscriptionId}";
                string resourceGroupName = "Development";
                string containerGroupName = "asd111"; // this group is short instances

                ResourceIdentifier resourceIdentifier = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
                ResourceGroupResource resourceGroup = armClient.GetResourceGroupResource(resourceIdentifier);
                ContainerGroupCollection containerGroupCollection = resourceGroup.GetContainerGroups();

                // 1. Set resource requirements using the correct types for 1.4.0
                var resourceRequests = new ContainerResourceRequestsContent(memoryInGB: 1, cpu: 1.0);
                var resourceRequirements = new ContainerResourceRequirements(resourceRequests);

                // 2. Initialize the container with the correct ContainerPort class

                string containerBaseName = $"test-container";
                string containerUniqueId = $"{DateTime.UtcNow:yyyyMMddHHmmssfffffff}";
                string containerName = $"{containerUniqueId}-{containerBaseName}";

                //"mcr.microsoft.com/azuredocs/aci-helloworld";
                string containerImage = dto.imageName;

                Console.WriteLine($"{nameof(containerName)}: {containerName}");
                Console.WriteLine($"{nameof(dto.imageName)}: {dto.imageName}");

                var container = new ContainerInstanceContainer(containerName, containerImage, resourceRequirements)
                {
                    // TODO: no port
                    // Fix: This must be 'ContainerPort' instead of 'ContainerGroupPort'
                    Ports = { new ContainerPort(80) }
                };

                // 5. Structure the global topology data for the Container Group
                var containerGroupData = new ContainerGroupData(
                    AzureLocation.EastUS,
                    new List<ContainerInstanceContainer> { container },
                    ContainerInstanceOperatingSystemType.Linux) // Explicit enum string wrapper used in 1.4.x
                {
                    // TODO: private and no port
                    IPAddress = new ContainerGroupIPAddress(
                        new List<ContainerGroupPort> { new ContainerGroupPort(80) },
                        ContainerGroupIPAddressType.Public)
                    {
                        DnsNameLabel = $"{containerBaseName}-{Guid.NewGuid().ToString().Substring(0, 8)}"
                    },
                    RestartPolicy = ContainerGroupRestartPolicy.Never, // NOTE: we don't want restart.

                };

                Console.WriteLine($"Deploying Container Group '{containerGroupName}' to Azure...");

                // 6. Invoke the resource deployment operation and await the API lifecycle confirmation
                ArmOperation<ContainerGroupResource> operation = await containerGroupCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed,
                    containerGroupName,
                    containerGroupData
                );

                ContainerGroupResource createdGroup = operation.Value;
                // This retrieves the unique ResourceIdentifier for the entire ACI deployment
                ResourceIdentifier groupResourceId = createdGroup.Id;


                Console.WriteLine("\nDeployment Successful!");
                Console.WriteLine($"FQDN: {createdGroup.Data.IPAddress.Fqdn}");
                Console.WriteLine($"IP Address: {createdGroup.Data.IPAddress.IP}");
                Console.WriteLine($"Container Group Azure Resource ID: {groupResourceId}");


                //Console.WriteLine($"[Publisher] Injected secret and launched container {containerId[..12]}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Publisher] Docker SDK Error: {ex}");
            }
        }

        public async Task LaunchReceiverContainerWithSdkAsyncWorksOnPublicRepoAndAcrWithPassword(WebhookPayload dto)
        {
            // 1. Initialize client using local 'az login' credentials

            //var managedIdentityId = ManagedIdentityId.FromUserAssignedObjectId("bc9df52a-c2b4-41f8-96a2-69054af16a6d");
            //var credentialOptions = new ManagedIdentityCredentialOptions(managedIdentityId)
            //{

            //};
            //var credential = new ManagedIdentityCredential(credentialOptions);
            //new DefaultAzureCredential(); // Only to run code locally.


            // 2. Define target Azure resource coordinates
            string subscriptionId = "{subscriptionId}";
            string resourceGroupName = "Development";
            string containerGroupName = "asd111"; // this group is short instances


            var credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
            var armClient = new ArmClient(credential);


            try
            {
                Console.WriteLine($"{nameof(Environment.UserDomainName)}:   {Environment.UserDomainName}");
                Console.WriteLine($"{nameof(Environment.UserName)}:         {Environment.UserName}");

                ResourceIdentifier resourceIdentifier = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
                ResourceGroupResource resourceGroup = armClient.GetResourceGroupResource(resourceIdentifier);
                ContainerGroupCollection containerGroupCollection = resourceGroup.GetContainerGroups();

                // 1. Set resource requirements using the correct types for 1.4.0
                var resourceRequests = new ContainerResourceRequestsContent(memoryInGB: 1, cpu: 1.0);
                var resourceRequirements = new ContainerResourceRequirements(resourceRequests);

                // 2. Initialize the container with the correct ContainerPort class

                string containerBaseName = $"test-container";
                string containerUniqueId = $"{DateTime.UtcNow:yyyyMMddHHmmssfffffff}";
                string containerName = $"{containerUniqueId}-{containerBaseName}";

                //"mcr.microsoft.com/azuredocs/aci-helloworld";
                string containerImage = dto.imageName;

                Console.WriteLine($"{nameof(containerName)}: {containerName}");
                Console.WriteLine($"{nameof(dto.imageName)}: {dto.imageName}");

                var container = new ContainerInstanceContainer(containerName, containerImage, resourceRequirements)
                {
                    // TODO: no port
                    // Fix: This must be 'ContainerPort' instead of 'ContainerGroupPort'
                    Ports = { new ContainerPort(80) }
                };

                // 3. Build the Image Registry block using the Factory (Avoids Obsolete compiler warnings)
                ContainerGroupImageRegistryCredential registryCredential = ArmContainerInstanceModelFactory.ContainerGroupImageRegistryCredential(
                    server: "development120260908161132.azurecr.io",
                    username: "development120260908161132", // Left null because we are using an Identity token, not a basic username
                    password: "{repoPassword}",
                   // identity: userAssignedIdentityId, // Directs ACI to use this identity token to authenticate
                    identityUri: null
                );

                // 5. Structure the global topology data for the Container Group
                var containerGroupData = new ContainerGroupData(
                    AzureLocation.EastUS,
                    new List<ContainerInstanceContainer> { container },
                    ContainerInstanceOperatingSystemType.Linux) // Explicit enum string wrapper used in 1.4.x
                {
                    // TODO: private and no port
                    IPAddress = new ContainerGroupIPAddress(
                        new List<ContainerGroupPort> { new ContainerGroupPort(80) },
                        ContainerGroupIPAddressType.Public)
                    {
                        DnsNameLabel = $"{containerBaseName}-{Guid.NewGuid().ToString().Substring(0, 8)}"
                    },
                    RestartPolicy = ContainerGroupRestartPolicy.Never, // NOTE: we don't want restart.
                    ImageRegistryCredentials = { registryCredential }

                    //ImageRegistryCredentials =
                    //    {
                    //        new ContainerGroupImageRegistryCredential("development120260908161132.azurecr.io", "development120260908161132")
                    //        {
                    //            Password = "G1NqzFYNW2YnSR8QNJaCrm55zdMGCd25YTa54DLT5ypikoaZ8bMvJQQJ99CIACYeBjFEqg7NAAACAZCRmXvK"
                    //        }
                    //    }
                };

                Console.WriteLine($"Deploying Container Group '{containerGroupName}' to Azure...");

                // 6. Invoke the resource deployment operation and await the API lifecycle confirmation
                ArmOperation<ContainerGroupResource> operation = await containerGroupCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed,
                    containerGroupName,
                    containerGroupData
                );

                ContainerGroupResource createdGroup = operation.Value;
                // This retrieves the unique ResourceIdentifier for the entire ACI deployment
                ResourceIdentifier groupResourceId = createdGroup.Id;


                Console.WriteLine("\nDeployment Successful!");
                Console.WriteLine($"FQDN: {createdGroup.Data.IPAddress.Fqdn}");
                Console.WriteLine($"IP Address: {createdGroup.Data.IPAddress.IP}");
                Console.WriteLine($"Container Group Azure Resource ID: {groupResourceId}");


                //Console.WriteLine($"[Publisher] Injected secret and launched container {containerId[..12]}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Publisher] Docker SDK Error: {ex}");
            }
        }

        public async Task LaunchReceiverContainerWithSdkAsyncWorksOnPublicRepoAndAcrManagedIdentity(WebhookPayload dto)
        {
            // 1. Initialize client using local 'az login' credentials

            //var managedIdentityId = ManagedIdentityId.FromUserAssignedObjectId("bc9df52a-c2b4-41f8-96a2-69054af16a6d");
            //var credentialOptions = new ManagedIdentityCredentialOptions(managedIdentityId)
            //{

            //};
            //var credential = new ManagedIdentityCredential(credentialOptions);
            //new DefaultAzureCredential(); // Only to run code locally.


            // 2. Define target Azure resource coordinates
            string subscriptionId = "{subscriptionId}";
            string resourceGroupName = "Development";
            string containerGroupName = "asd111"; // this group is short instances
            string userAssignedIdentity = "development_aca_pool";


            var credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
            var armClient = new ArmClient(credential);
            // The resource ID of a User-Assigned Managed Identity that has the 'AcrPull' role on your ACR
            string userAssignedIdentityId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{userAssignedIdentity}";


            try
            {
                Console.WriteLine($"{nameof(Environment.UserDomainName)}:   {Environment.UserDomainName}");
                Console.WriteLine($"{nameof(Environment.UserName)}:         {Environment.UserName}");

                ResourceIdentifier resourceIdentifier = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
                ResourceGroupResource resourceGroup = armClient.GetResourceGroupResource(resourceIdentifier);
                ContainerGroupCollection containerGroupCollection = resourceGroup.GetContainerGroups();

                // 1. Set resource requirements using the correct types for 1.4.0
                var resourceRequests = new ContainerResourceRequestsContent(memoryInGB: 1, cpu: 1.0);
                var resourceRequirements = new ContainerResourceRequirements(resourceRequests);

                // 2. Initialize the container with the correct ContainerPort class

                string containerBaseName = $"test-container";
                string containerUniqueId = $"{DateTime.UtcNow:yyyyMMddHHmmssfffffff}";
                string containerName = $"{containerUniqueId}-{containerBaseName}";

                //"mcr.microsoft.com/azuredocs/aci-helloworld";
                string containerImage = dto.imageName;

                Console.WriteLine($"{nameof(containerName)}: {containerName}");
                Console.WriteLine($"{nameof(dto.imageName)}: {dto.imageName}");

                var container = new ContainerInstanceContainer(containerName, containerImage, resourceRequirements)
                {
                    // TODO: no port
                    // Fix: This must be 'ContainerPort' instead of 'ContainerGroupPort'
                    Ports = { new ContainerPort(80) }
                };

                // 3. Build the Image Registry block using the Factory (Avoids Obsolete compiler warnings)
                ContainerGroupImageRegistryCredential registryCredential = ArmContainerInstanceModelFactory.ContainerGroupImageRegistryCredential(
                    server: "development120260908161132.azurecr.io",
                    username: null, // Left null because we are using an Identity token, not a basic username
                    password: null,
                    identity: userAssignedIdentityId, // Directs ACI to use this identity token to authenticate
                    identityUri: null
                );

                // 5. Structure the global topology data for the Container Group
                var containerGroupData = new ContainerGroupData(
                    AzureLocation.EastUS,
                    new List<ContainerInstanceContainer> { container },
                    ContainerInstanceOperatingSystemType.Linux) // Explicit enum string wrapper used in 1.4.x
                {
                    // TODO: private and no port
                    IPAddress = new ContainerGroupIPAddress(
                        new List<ContainerGroupPort> { new ContainerGroupPort(80) },
                        ContainerGroupIPAddressType.Public)
                    {
                        DnsNameLabel = $"{containerBaseName}-{Guid.NewGuid().ToString().Substring(0, 8)}"
                    },
                    RestartPolicy = ContainerGroupRestartPolicy.Never, // NOTE: we don't want restart.

                    // Assign the identity to the Container Group architecture
                    Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.UserAssigned)
                    {
                        UserAssignedIdentities = { { new ResourceIdentifier(userAssignedIdentityId), new UserAssignedIdentity() } }
                    },

                    // Assign our factory-generated credential block
                    ImageRegistryCredentials = { registryCredential }
                };

                Console.WriteLine($"Deploying Container Group '{containerGroupName}' to Azure...");

                // 6. Invoke the resource deployment operation and await the API lifecycle confirmation
                ArmOperation<ContainerGroupResource> operation = await containerGroupCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed,
                    containerGroupName,
                    containerGroupData
                );

                ContainerGroupResource createdGroup = operation.Value;
                // This retrieves the unique ResourceIdentifier for the entire ACI deployment
                ResourceIdentifier groupResourceId = createdGroup.Id;


                Console.WriteLine("\nDeployment Successful!");
                Console.WriteLine($"FQDN: {createdGroup.Data.IPAddress.Fqdn}");
                Console.WriteLine($"IP Address: {createdGroup.Data.IPAddress.IP}");
                Console.WriteLine($"Container Group Azure Resource ID: {groupResourceId}");


                //Console.WriteLine($"[Publisher] Injected secret and launched container {containerId[..12]}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Publisher] Docker SDK Error: {ex}");
            }
        }
    
        public async Task NewNotTested()
        {
            // 1. Initialise the base Client
            ArmClient client = new ArmClient(new DefaultAzureCredential());

            string subscriptionId = "your-subscription-id";
            string resourceGroupName = "your-resource-group";
            string identityName = "myAcrPullIdentity";
            string acrName = "myregistry"; // Your ACR Name (without .azurecr.io)
            string acrServer = $"{acrName}.azurecr.io";

            ResourceIdentifier rgId = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
            ResourceGroupResource resourceGroup = client.GetResourceGroupResource(rgId);

            // 2. Proactively create the User-Assigned Managed Identity
            UserAssignedIdentityCollection identityCollection = resourceGroup.GetUserAssignedIdentities();
            var identityOperation = await identityCollection.CreateOrUpdateAsync(WaitUntil.Completed, identityName, new UserAssignedIdentityData(AzureLocation.AustraliaEast));
            UserAssignedIdentityResource identity = identityOperation.Value;

            Guid principalId = identity.Data.PrincipalId.Value;
            ResourceIdentifier identityResourceId = identity.Data.Id;

            // 3. Dynamically apply the AcrPull Role Assignment to the Identity over your ACR scope
            // "7f951dda-4ed3-4680-a7ca-43fe172d538d" is the global Azure RBAC fixed definition ID for 'AcrPull'
            ResourceIdentifier acrPullRoleDefinitionId = new ResourceIdentifier($"/subscriptions/{subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d");
            ResourceIdentifier acrScope = new ResourceIdentifier($"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{acrName}");

//            RoleAssignmentCollection roleAssignments = client.GetRoleAssignments(acrScope);
//            string uniqueAssignmentName = Guid.NewGuid().ToString(); // Role assignments require a deterministic or unique GUID name

//            RoleAssignmentCreateOrUpdateContent roleContent = new RoleAssignmentCreateOrUpdateContent(acrPullRoleDefinitionId, principalId)
//            {
//                PrincipalType = RoleManagementPrincipalType.ServicePrincipal
//            };

//            // Execute role mapping
//            await roleAssignments.CreateOrUpdateAsync(WaitUntil.Completed, uniqueAssignmentName, roleContent);
//            Console.WriteLine("Successfully mapped AcrPull role permissions to Managed Identity.");

//            // 3. Define the Environment Variables via safe Model Factories
//            var envVariables = new List<ContainerEnvironmentVariable>
//{
//    // A standard public environment variable
//    // read with Environment.GetEnvironmentVariable("APP_ENVIRONMENT")
//    ArmContainerInstanceModelFactory.ContainerEnvironmentVariable(
//        name: "APP_ENVIRONMENT",
//        value: "Production",
//        secureValue: "lol"
//    ),

//    // A secure environment variable (hidden from logs and Azure Portal)
    
//    // read with Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//    ArmContainerInstanceModelFactory.ContainerEnvironmentVariable(
//        name: "DATABASE_CONNECTION_STRING",
//        value: null, // Must be null when setting a secure value
//        secureValue: "Server=tcp:myserver.database.windows.net..."
//    )
//};

//            // 4. Construct Container Specifications via SDK Model Factories
//            ContainerInstanceContainer containerSpec = ArmContainerInstanceModelFactory.ContainerInstanceContainer(
//                name: "my-app-container",
//                image: $"{acrServer}/my-app-image:latest",
//                resources: ArmContainerInstanceModelFactory.ContainerResourceRequirements(
//                    requests: ArmContainerInstanceModelFactory.ContainerResourceRequests(memoryInGB: 1.5, cpu: 1.0)
//                ),
//                environmentVariables: envVariables
//            );

//            // Map registry using the safe Model Factory method (Avoids compiler warnings)
//            ContainerGroupImageRegistryCredential registryCredential = ArmContainerInstanceModelFactory.ContainerGroupImageRegistryCredential(
//                server: acrServer,
//                username: null,
//                password: null,
//                identity: identityResourceId.ToString(),
//                identityUri: null
//            );

//            // 5. Complete deployment package payload
//            ContainerGroupCollection containerGroups = resourceGroup.GetContainerGroups();
//            ContainerGroupData containerGroupData = new ContainerGroupData(
//                AzureLocation.AustraliaEast,
//                new[] { containerSpec },
//                ContainerGroupOperatingSystemType.Linux
//            )
//            {
//                RestartPolicy = ContainerGroupRestartPolicy.Always,
//                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.UserAssigned)
//                {
//                    UserAssignedIdentities = { { identityResourceId, new UserAssignedIdentity() } }
//                },
//                ImageRegistryCredentials = { registryCredential }
//            };

            // 6. Deploy & Spin up
            //var containerOperation = await containerGroups.CreateOrUpdateAsync(WaitUntil.Completed, "my-secure-container-group", containerGroupData);
           // Console.WriteLine($"Container running completely passwordless! ID: {containerOperation.Value.Data.Id}");

        }
    }
}
