using Docker.DotNet;
using Docker.DotNet.Models;
using System.Runtime.InteropServices;
using System.Text;

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - starting...");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/webhook", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    if(string.IsNullOrWhiteSpace(body))
    {
        body = "No payload provided! Hello from starter into Docker STDIN!";
    }

    Console.WriteLine($"[Publisher] Received webhook for with payload:{Environment.NewLine}{body}");
    Console.WriteLine("Launching container via SDK...");

    await LaunchReceiverContainerWithSdkAsync(body);

    Console.WriteLine("Container launched...");

    return Results.Accepted();
});

async Task LaunchReceiverContainerWithSdkAsync(string payload)
{
    Uri dockerUri;

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        // Windows local pipe
        dockerUri = new Uri("npipe://./pipe/docker_engine");
    }
    else
    {
        // Linux/macOS Unix Socket file
        dockerUri = new Uri("unix:///var/run/docker.sock");
    }

    var clientConfig = new DockerClientBuilder();
     var client = clientConfig
                     .WithEndpoint(dockerUri)
                     .Build();

    try
    {
        var version = await client.System.GetVersionAsync();
        Console.WriteLine($"Connected to Docker! Version: {version.Version}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
    }

    const string imageName = "dynamic-receiver-piper";

    try
    {
        // 1. CRITICAL FLAGS: OpenStdin and StdinOnce must be TRUE
        var createParams = new CreateContainerParameters
        {
            Image = imageName,
            AttachStdin = true,  // Attach input channels
            OpenStdin = true,    // Keep stdin channel open
            StdinOnce = true,    // Close stdin automatically when sender finishes
            Tty = false,         // Must be false for binary streams
            HostConfig = new HostConfig { AutoRemove = true }
        };

        Console.WriteLine("Creating container...");
        var response = await client.Containers.CreateContainerAsync(createParams);
        var containerId = response.ID;
        Console.WriteLine($"Container created. {containerId[..12]}");

        // 2. Attach to the container's streams BEFORE starting it so we don't miss the window
        Console.WriteLine("Attaching to streams...");
        var stream = await client.Containers.AttachContainerAsync(containerId, new ContainerAttachParameters
        {
            Stdin = true,
            Stream = true
        }, default).ConfigureAwait(false);

        // 5. Start the container
        Console.WriteLine("Starting container...");
        await client.Containers.StartContainerAsync(response.ID, default).ConfigureAwait(false);

        // 6. Pipe data into the container's STDIN
        var dataToPipe = $"{payload}{Environment.NewLine}";
        var buffer = Encoding.UTF8.GetBytes(dataToPipe);

        Console.WriteLine("Writing data to container STDIN...");
        await stream.WriteAsync(buffer, 0, buffer.Length, default).ConfigureAwait(false);

        /*
            This is required signal to receiver that the data write is complete. 
            Prevents receiver reads forever. Console.OpenStandardInput() reads forever.
         */

        stream.Dispose(); 

        Console.WriteLine($"[Publisher] Injected secret and launched container {containerId[..12]}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Publisher] Docker SDK Error: {ex}");
    }
}

Console.WriteLine("PublisherDockerDotNetPiperConsole1 - started...");

app.Run();
