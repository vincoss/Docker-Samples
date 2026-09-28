using SampleAzureContainerController.Dto;
using SampleAzureContainerController.Interface;


namespace SampleAzureContainerController.Services
{
    public class ProcessingService : IProcessingService
    {
        public Task RunAsync(WebhookPayloadDto dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
