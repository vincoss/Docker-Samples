using System.Diagnostics;
using System.Text.Json;


// run with cmd
// curl -X POST http://localhost:5000/webhook -H "Content-Type: application/json" -d "{\"TenantId\":\"client-alpha\", \"EventType\":\"push\"}"

Console.WriteLine("PublisherConsole1 - Hello, World!");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Webhook endpoint
app.MapPost("/webhook", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Parse the payload to determine the target tenant or database
    var payload = JsonSerializer.Deserialize<WebhookPayload>(body);
    if (payload == null || string.IsNullOrEmpty(payload.TenantId))
    {
        return Results.BadRequest("Invalid payload");
    }

    // Dynamic Logic: Resolve the unique SQL connection string based on the webhook event
    string dynamicSqlString = ResolveSqlStringForTenant(payload.TenantId);

    Console.WriteLine($"[Publisher] Received webhook for {payload.TenantId}. Launching serverless container...");

    // Agnostic Execution: Spin up the receiver container, injecting the secret dynamically
    LaunchReceiverContainer(dynamicSqlString);

    return Results.Accepted();
});

string ResolveSqlStringForTenant(string tenantId)
{
    return $"Server=tcp:{tenantId}.database.windows.net;Database=ProductionDB;User Id=AppUser;";
}

void LaunchReceiverContainer(string sqlConnectionString)
{
    // Spins up the Docker container on demand.
    // The key is "-e SQL_CONNECTION_STRING=...", which injects the variable into the isolated runtime env.
    var startInfo = new ProcessStartInfo
    {
        FileName = "docker",
        Arguments = $"run dynamic-receiver -e SOME_ARG=\"{sqlConnectionString}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = new Process { StartInfo = startInfo };

    // Simple async logging for illustration
    process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
    process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
}

app.Run();

// Minimal payload structure matching the webhook data
public record WebhookPayload(string TenantId, string EventType);
