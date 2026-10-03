using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SampleTests
{
    public class Tests
    {
        public record WebhookPayloadDto(JsonElement JobData, string JobContainerName, string? ImageName);

        [Fact]
        public void Test()
        {
            JsonElement emptyUndefined = JsonElement.Parse("{}");
            var payload = new WebhookPayloadDto(emptyUndefined, "sample-container", "sample-image");

            var str = JsonSerializer.Serialize(payload);
        }
    }
}
