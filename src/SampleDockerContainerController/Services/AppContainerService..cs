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
                Tty =  true,
                HostConfig = new HostConfig { AutoRemove = true }
            };

            Console.WriteLine("Creating container...");
            var response = await client.Containers.CreateContainerAsync(createParams);
            var containerId = response.ID;
            Console.WriteLine($"Container created. {containerId[..12]}");

            // 5. Start the container
            Console.WriteLine("Starting container...");
            await client.Containers.StartContainerAsync(response.ID, default).ConfigureAwait(false);
        }
    }
}