using SampleAzureContainerController.Dto;


namespace SampleAzureContainerController.Interface
{
    public interface IAppContainerService
    {
        Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken);
    }
}
