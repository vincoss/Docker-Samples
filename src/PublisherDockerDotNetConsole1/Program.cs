using System.Text.Json;
using Docker.DotNet;
using Docker.DotNet.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/webhook", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    //var payload = JsonSerializer.Deserialize<WebhookPayload>(body);

    var payload = new WebhookPayload("A0001", "test");

    if (payload == null || string.IsNullOrEmpty(payload.TenantId))
    {
        return Results.BadRequest("Invalid payload");
    }

    string dynamicSqlString = ResolveSqlStringForTenant(payload.TenantId);

    Console.WriteLine($"[Publisher] Received webhook for {payload.TenantId}. Launching container via SDK...");

    // Call our updated SDK method asynchronously
    await LaunchReceiverContainerWithSdkAsync(dynamicSqlString);

    return Results.Accepted();
});

string ResolveSqlStringForTenant(string tenantId)
{
    return $"Server=tcp:{tenantId}.database.windows.net;Database=ProductionDB;User Id=AppUser;";
}

async Task LaunchReceiverContainerWithSdkAsync(string sqlConnectionString)
{
    // 1. Initialize the client using standard OS defaults (pipe on Windows / socket on Linux)
    using var clientConfig = new DockerClientConfiguration();
    using var client = clientConfig.CreateClient();

    const string imageName = "dynamic-receiver";

    try
    {
        // 2. Configure container parameters and inject the secret via the 'Env' list
        var createParams = new CreateContainerParameters
        {
            Image = imageName,
            Env = new List<string>
            {
                $"SQL_CONNECTION_STRING={sqlConnectionString}"
            },
            Cmd = new List<string>
            {
                $"SOME_ARG={sqlConnectionString}"
            },
            // Automatically clean up the container resources after it finishes running (--rm)
            HostConfig = new HostConfig
            {
                AutoRemove = false
            }
        };

        // 3. Request Docker to create the container instance
        var response = await client.Containers.CreateContainerAsync(createParams);
        string containerId = response.ID;

        // 4. Fire up the container execution pipeline
        await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
        Console.WriteLine($"[Publisher] Container {containerId[..12]} started successfully.");

        // Optional: Stream logs from the spawned container directly into the host console output
        MultiplexedStream logStream = await client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = true
        });

        var buffer = new byte[4096];
        while (true)
        {
            // Read Next Multiplexed Block
            var readResult = await logStream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);

            if (readResult.Count == 0)
                break; // Stream ended

            // Convert the raw block into text and print it
            string logLine = System.Text.Encoding.UTF8.GetString(buffer, 0, readResult.Count);
            Console.Write(logLine);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Publisher] Docker SDK Error: {ex.Message}");
    }
}

app.Run();

public record WebhookPayload(string TenantId, string EventType);
