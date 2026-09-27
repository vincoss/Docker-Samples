using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDotNetTestcontainersContainerController
{
    public class DotNetTestcontainersSample
    {
        public async Task RunLinuxAsync()
        {
            // 1. Configure the container as a one-shot job
            var jobContainer = new ContainerBuilder("alpine:latest")
                .WithCommand("sh", "-c", "echo 'Running job...' && sleep 2 && echo 'Job completed!'")
                // Wait until a specific log message appears, and treat the exit as successful
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilMessageIsLogged("Job completed!", options => options.WithMode(WaitStrategyMode.OneShot)))
                .Build();

            // 2. Start the container (this waits until the wait strategy completes)
            await jobContainer.StartAsync();

            // 3. (Optional) Inspect the execution result
            long exitCode = await jobContainer.GetExitCodeAsync();
            Console.WriteLine($"Job finished with exit code: {exitCode}");
        }

        public async Task RunWindowsAsync()
        {
            // 1. Define and build the container configuration
            var container = new ContainerBuilder("hello-world")
                .WithCleanUp(true)         // Automatically removes container when disposed
                .Build();

            // 2. Start the container
            await container.StartAsync();

            Console.WriteLine("Container is running!");

            // 3. Stop and clean up when done
            await container.StopAsync();
        }
    }
}
