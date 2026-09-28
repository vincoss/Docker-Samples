using SampleAzureContainerController.Dto;

namespace SampleAzureContainerController.Interface
{
    public interface IProcessingService
    {
        Task RunAsync(WebhookPayloadDto dto, CancellationToken cancellationToken);
    }
}
