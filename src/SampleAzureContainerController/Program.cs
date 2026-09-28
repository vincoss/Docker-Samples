using Microsoft.AspNetCore.Mvc;
using SampleAzureContainerController.Dto;
using SampleAzureContainerController.Interface;
using SampleAzureContainerController.Services;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<QueueBackgroundService>();
builder.Services.AddSingleton<IAppContainerService, AppContainerService>();
builder.Services.AddSingleton<IProcessingService, ProcessingService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/webhook", async (HttpContext context, [FromServices] IProcessingService processingService, CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(body))
    {
        Console.Error.WriteLine("Request body is required");
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
    if (payload == null || string.IsNullOrWhiteSpace(payload.jobContainerName))
    {
        Console.Error.WriteLine("Missing jobContainerName inside payload.");
        return Results.BadRequest("Missing required jobContainerName property.");
    }

    Console.WriteLine("Launching container via Azure SDK...");

    await processingService.RunAsync(payload, cancellationToken);

    Console.WriteLine($"Container [{payload.jobContainerName}] execution request completed...");

    return Results.Accepted();
});

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();
