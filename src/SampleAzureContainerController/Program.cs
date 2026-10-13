using Microsoft.AspNetCore.Mvc;
using SampleAzureContainerController.Dto;
using SampleAzureContainerController.Interface;
using SampleAzureContainerController.Services;
using System.Text.Json;

// dotnet SampleAzureContainerController.dll resourceGroupName=development

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton<IAppContainerService>(provider =>
{
    var resourceGroupName = builder.Configuration["resourceGroupName"];

    if(string.IsNullOrWhiteSpace(resourceGroupName))
    {
        throw new ArgumentNullException(nameof(resourceGroupName));
    }

    Console.WriteLine($"Building {nameof(AppContainerService)} with resource group: {resourceGroupName}");

    var logger = provider.GetRequiredService<ILogger<AppContainerService>>();

    return new AppContainerService(resourceGroupName, logger);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/webhook", async (HttpContext context, [FromServices] IAppContainerService appContainerService, CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(body))
    {
        Console.Error.WriteLine("Request body is required!");
        return Results.BadRequest();
    }

    Console.WriteLine($"[Publisher] Received webhook for with payload:{Environment.NewLine}{body}");

    WebhookPayloadDto? payload;
    try
    {
        payload = JsonSerializer.Deserialize<WebhookPayloadDto>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    catch (JsonException ex)
    {
        Console.Error.WriteLine($"Failed to deserialize payload: {ex.Message}");
        return Results.BadRequest("Invalid JSON structure.");
    }

    // Validate the core required identifiers
    if (payload == null || string.IsNullOrWhiteSpace(payload.JobContainerName))
    {
        Console.Error.WriteLine("Missing jobContainerName inside payload.");
        return Results.BadRequest("Missing required jobContainerName property.");
    }

    Console.WriteLine("Launching container via Azure SDK...");

    var jobData = GetJob(payload);
    await appContainerService.StartAsync(jobData, cancellationToken);

    Console.WriteLine($"Container [{payload.JobContainerName}] execution request completed...");

    return Results.Accepted();
});

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();

static ContainerJobDto GetJob(WebhookPayloadDto job)
{
    var contarnerJob = new ContainerJobDto
    {
        Name = job.JobContainerName,
        ImageName = job.ImageName
    };

    contarnerJob.Envs.Add(new ContainerJobDto.EnvironmentVariable
    {
        Name = nameof(WebhookPayloadDto.JobData),
        Value = job.JobData.ToString()
    });
    
    return contarnerJob;
}