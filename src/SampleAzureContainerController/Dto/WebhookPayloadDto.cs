using System.Text.Json;


namespace SampleAzureContainerController.Dto
{
    public record WebhookPayloadDto(JsonElement JobData, string JobContainerName, string? ImageName);
}
