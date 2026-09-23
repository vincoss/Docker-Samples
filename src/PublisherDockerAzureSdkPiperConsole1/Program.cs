using PublisherDockerAzureSdkPiperConsole1;
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

    await new AcaContainerProvisioning().LaunchReceiverContainerWithSdkAsyncExisting(dto);

    Console.WriteLine("Container launched...");

    return Results.Accepted();
});

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();

public record WebhookPayload(string imageName);