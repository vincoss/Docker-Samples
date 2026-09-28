using System.Text.Json;

namespace SampleAzureContainerController.Dto
{
    public record WebhookPayloadDto(JsonElement jobData, string jobContainerName, string? imageName);
}
