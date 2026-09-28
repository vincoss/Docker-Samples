using System;


namespace SampleAzureContainerController.Dto
{
    public class ContainerJobDto
    {
        public required string Name { get; set; }

        public required string ImageName { get; set; }

        public IList<string> Args { get;} = new List<string>();

        public IList<string> Commands { get; } = new List<string>();

        public IList<EnvironmentVariable> Envs { get; } = new List<EnvironmentVariable>();

        public class EnvironmentVariable
        {
            public required string Name { get; set; }

            public string? Value { get; set; }

            public string? SecretRef { get; set; }
        }
    }
}