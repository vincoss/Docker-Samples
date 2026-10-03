using System.Text.Json;


namespace SampleDockerContainerController.Dto
{
    public record WebhookPayloadDto(JsonElement JobData, string JobContainerName, string? ImageName);
}
