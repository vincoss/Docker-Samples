using Docker.DotNet;
using Docker.DotNet.Models;
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

            _logger.LogDebug(nameof(StartAsync));

            var clientConfig = new DockerClientBuilder();
            var client = clientConfig.Build();

            _logger.LogInformation("Compiling job configuration overrides for container job: [{JobName}]", job.ImageName);

            var containerOverride = new CreateContainerParameters
            {
                Image = job.ImageName,
                StopTimeout = TimeSpan.FromMinutes(1800),
                HostConfig = new HostConfig { AutoRemove = true }
            };

            // Add args
            foreach (var arg in job.Args)
            {
                containerOverride.Cmd.Add(arg);
            }

            // Envs
            if(containerOverride.Env == null)
            {
                containerOverride.Env = new List<string>();
            }

            foreach (var env in job.Envs)
            {
                containerOverride.Env.Add($"{env.Name.Trim()}={env.Value?.Trim() ?? env.SecretRef?.Trim()}");
            }

            foreach (var cmd in job.Commands)
            {
                containerOverride.Entrypoint.Add(cmd);
            }

            _logger.LogInformation("Dispatching trigger request to create container: [{JobName}]", job.ImageName);

            var response = await client.Containers.CreateContainerAsync(containerOverride, cancellationToken).ConfigureAwait(false);
            var containerId = response.ID;
            _logger.LogInformation($"Container created. {containerId[..12]}");

            // Start the container
            _logger.LogInformation("Starting container...");
            var result = await client.Containers.StartContainerAsync(response.ID, cancellationToken: cancellationToken).ConfigureAwait(false);

            if(result)
            {
                _logger.LogInformation("Successfully requested Azure to start job [{JobName}].", response.ID);
            }
            else
            {
                _logger.LogInformation("Failed start container [{JobName}].", response.ID);
            }
        }
    }
}