using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Xunit;


namespace SampleTests
{
    public class EnvironmentVariablesTest
    {
        class AppConfig
        {
            public required string Project {  get; set; }
            public required string SiteId { get; set; }

            public required ModelSetting ModelSettings {  get; set; }

            public class ModelSetting
            {
                public string Fresh { get; set; } = string.Empty;
                public string Weath { get; set; } = string.Empty;
            }
        }

        public static string GetConfigJson()
        {
            var data = new 
            {
                Project = "ASD",
                SiteId = "Y123",
                ModelSettings = new
                {
                    Fresh = "AllProjectsFresh.joblib",
                    Weath = "AllProjectsWeath.joblib"
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(data, options);
        }

        [Fact]
        public void ToProcessArgumentsTest()
        {
            string incomingJson = """
            {
                "Project": "ASD",
                "SiteId": "Y123",
                "ModelSettings": {
                    "Fresh": "AllProjectsFresh.joblib",
                    "Weath": "AllProjectsWeath.joblib"
                }
            }
            """;

            using var jsonDoc = JsonDocument.Parse(incomingJson);
            var args = jsonDoc.ToProcessArguments(delimiter: ":").ToArray();

            var builder = Host.CreateApplicationBuilder(args);

            var myConfig = builder.Configuration.Get<AppConfig>(options =>
            {
                options.BindNonPublicProperties = true; // Force bind properties case-insensitively
            });

            Assert.NotNull(myConfig);
            Assert.NotNull(myConfig.ModelSettings);
            Assert.Equal("ASD", myConfig.Project);
            Assert.Equal("Y123", myConfig.SiteId);
            Assert.Equal("AllProjectsFresh.joblib", myConfig.ModelSettings.Fresh);
            Assert.Equal("AllProjectsWeath.joblib", myConfig.ModelSettings.Weath);
        }

        [Fact]
        public void ToEnvironmentVariablesTest()
        {
            string incomingJson = """
            {
                "Project": "ASD",
                "SiteId": "Y123",
                "ModelSettings": {
                    "fresh": "AllProjectsFresh.joblib",
                    "weath": "AllProjectsWeath.joblib"
                }
            }
            """;

            using var doc = JsonDocument.Parse(incomingJson);

            var envVariables = doc.ToEnvironmentVariables(prefix: "APP_", delimiter: "__").ToArray();

            Assert.NotEmpty(envVariables);

            var projectVar = envVariables.FirstOrDefault(e => e.Name == "APP_Project");
            Assert.NotNull(projectVar);
            Assert.Equal("ASD", projectVar.Value);

            var freshVar = envVariables.FirstOrDefault(e => e.Name == "APP_ModelSettings__fresh");
            Assert.NotNull(freshVar);
            Assert.Equal("AllProjectsFresh.joblib", freshVar.Value);
        }

        [Fact]
        public void Test_Configuration_Binding_With_Correct_Casing()
        {
            string incomingJson = """
            {
                "Project": "ASD",
                "SiteId": "Y123",
                "ModelSettings": {
                    "Fresh": "AllProjectsFresh.joblib",
                    "Weath": "AllProjectsWeath.joblib"
                }
            }
            """;

            using var jsonDoc = JsonDocument.Parse(incomingJson);

            // Use the colon (:) delimiter for command-line arguments, without a prefix,
            // so it binds directly to the root of your AppConfig type.
            var args = jsonDoc.ToProcessArguments(prefix: "", delimiter: ":").ToArray();

            var builder = Host.CreateApplicationBuilder(args);
            var myConfig = builder.Configuration.Get<AppConfig>();

            Assert.NotNull(myConfig);
            Assert.NotNull(myConfig.ModelSettings);
            Assert.Equal("ASD", myConfig.Project);
            Assert.Equal("Y123", myConfig.SiteId);
            Assert.Equal("AllProjectsFresh.joblib", myConfig.ModelSettings.Fresh);
            Assert.Equal("AllProjectsWeath.joblib", myConfig.ModelSettings.Weath);
        }
    }
    public class EnvironmentVariable
    {
        public required string Name { get; set; }
        public string? Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }

    public static class JsonEnvironmentExtensions
    {
        public static IEnumerable<EnvironmentVariable> ToEnvironmentVariables(
            this JsonDocument document,
            string prefix = "",
            string delimiter = "__")
        {
            var envVars = new List<EnvironmentVariable>();
            // Pass prefix and currentPath separately so the delimiter isn't appended at the root
            FlattenJsonElement(document.RootElement, prefix, string.Empty, delimiter, envVars);
            return envVars;
        }

        public static IEnumerable<string> ToProcessArguments(
            this JsonDocument document,
            string prefix = "",
            string delimiter = "__",
            bool forceUppercase = false)
        {
            var envVars = document.ToEnvironmentVariables(prefix, delimiter);
            return envVars.Select(envVar => $"{envVar.Name}={envVar.Value ?? string.Empty}");
        }

        private static void FlattenJsonElement(
            JsonElement element,
            string prefix,
            string currentPath,
            string delimiter,
            List<EnvironmentVariable> results)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string propertyName = property.Name;

                        // Delimiter is ONLY used when appending nested children down the hierarchy tree
                        string nextPath = string.IsNullOrEmpty(currentPath)
                            ? propertyName
                            : $"{currentPath}{delimiter}{propertyName}";

                        FlattenJsonElement(property.Value, prefix, nextPath, delimiter, results);
                    }
                    break;

                case JsonValueKind.Array:
                    results.Add(new EnvironmentVariable { Name = $"{prefix}{currentPath}", Value = element.GetRawText() });
                    break;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    results.Add(new EnvironmentVariable { Name = $"{prefix}{currentPath}", Value = null });
                    break;

                default:
                    // Prepend the prefix to the finalized flattened key path here
                    results.Add(new EnvironmentVariable { Name = $"{prefix}{currentPath}", Value = element.ToString() });
                    break;
            }
        }
    }



}
