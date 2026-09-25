using SampleContainerController.Dto;


namespace SampleContainerController.Interface
{
    public interface IAppContainerService
    {
        Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken);
    }
}
