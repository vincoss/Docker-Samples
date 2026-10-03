using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using SampleDockerContainerController.Dto;
using SampleDockerContainerController.Interface;


namespace SampleDockerContainerController.Services
{
    public class AppContainerService : IAppContainerService
    {
        private readonly ILogger<AppContainerService> _logger;

        public AppContainerService(ILogger<AppContainerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            var clientConfig = new DockerClientBuilder();
            var client = clientConfig.Build();

            var createParams = new CreateContainerParameters
            {
                Image = job.ImageName,
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
                Stdout = true,
                Stderr = true,
                Logs = true
            }, default).ConfigureAwait(false);

            // 5. Start the container
            Console.WriteLine("Starting container...");
            await client.Containers.StartContainerAsync(response.ID, default).ConfigureAwait(false);


            var buffer = new byte[4096];
            while (true)
            {
                // Read Next Multiplexed Block
                var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);

                if (readResult.Count == 0)
                    break; // Stream ended

                // Convert the raw block into text and print it
                string logLine = System.Text.Encoding.UTF8.GetString(buffer, 0, readResult.Count);
                Console.Write(logLine);
            }
        }
    }
}