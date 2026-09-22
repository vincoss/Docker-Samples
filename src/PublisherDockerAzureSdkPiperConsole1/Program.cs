using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Provisioning.ContainerInstance;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Mvc.Formatters;
using PublisherDockerAzureSdkPiperConsole1;
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

    await new AciContainerProvisioning().LaunchReceiverContainerWithSdkAsyncWorksOnPublicRepoAndAcrManagedIdentity(dto);

    Console.WriteLine("Container launched...");

    return Results.Accepted();
});

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();

public record WebhookPayload(string imageName);