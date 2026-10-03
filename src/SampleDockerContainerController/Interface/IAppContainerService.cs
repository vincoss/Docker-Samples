using SampleDockerContainerController.Dto;


namespace SampleDockerContainerController.Interface
{
    public interface IAppContainerService
    {
        Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken);
    }
}
