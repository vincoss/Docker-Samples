using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Azure.ResourceManager.ContainerInstance.Models;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/webhook", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(body))
    {
        Console.Error.WriteLine("Request body is required");
        return Results.BadRequest();
    }

    Console.WriteLine($"[Publisher] Received webhook for with payload:{Environment.NewLine}{body}");

    var dto = JsonSerializer.Deserialize<WebhookPayload>(body);

    Console.WriteLine("Launching container via Azure SDK...");

    await LaunchReceiverContainerWithSdkAsync(dto);

    Console.WriteLine("Container launched...");

    return Results.Accepted();
});

async Task LaunchReceiverContainerWithSdkAsync(WebhookPayload dto)
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
        string subscriptionId = "";
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

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();

public record WebhookPayload(string imageName);