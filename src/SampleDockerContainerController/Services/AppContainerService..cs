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

            throw new NotImplementedException();
        }
    }
}