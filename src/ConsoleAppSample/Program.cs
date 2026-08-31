using ConsoleAppSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NReco.Logging.File;
using System;
using System.Text;

Console.WriteLine($"Args: {string.Join(",", args)}");

var baseDataDirectory = AppContext.BaseDirectory;
var appData = baseDataDirectory.Replace("app", "appdata");
var logsPathPattern = Path.Combine(appData, "logs", "ConsoleAppSample-{0:yyyy}-{0:MM}-{0:dd}.txt");

Console.WriteLine($"{nameof(AppContext.BaseDirectory)}: {baseDataDirectory}");
Console.WriteLine($"AppData:    {appData}");
Console.WriteLine($"Logs Path:  {logsPathPattern}");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddLogging(builder =>
    {
        builder.AddConfiguration(config.GetSection("Logging"));
        builder.AddConsole();
        builder.AddFile(logsPathPattern, options =>
        {
            options.FormatLogFileName = fileNameTemplate =>
            {
                return string.Format(fileNameTemplate, DateTime.UtcNow);
            };

            options.FormatLogEntry = (msg) => 
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
                var exceptionStr = msg.Exception != null ? $"\n{msg.Exception}" : "";

                return $"[{timestamp}] [{msg.LogLevel}] {msg.LogName}: {msg.Message}{exceptionStr}";
            };

            options.Append = true;
        });
    })
    .BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Application started successfully.");
logger.LogWarning("This is a warning log message.");

logger.LogInformation(MachineInfoUtility.PrintSystemSummary());

try
{
    throw new InvalidOperationException("Oops! Something went wrong.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An unhandled exception occurred.");
}

Console.WriteLine("Hello, ConsoleAppSample!");
