namespace SampleAzureContainerController.Services
{
    public class QueueBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background service has started.");

            // Loop runs continuously until the application shuts down
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Worker running at: {UTCtime}", DateTimeOffset.UtcNow);

                // Simulate work by delaying for 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("Background service is stopping.");
        }
    }
}