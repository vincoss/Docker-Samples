using Microsoft.Extensions.Configuration;

Console.WriteLine("ReceiverConsole1 - Hello, World!");
Console.WriteLine($"args: {string.Join(",", args)}");

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()  
    .AddCommandLine(args)
    .Build();

// Fetch the dynamic connection string injected by the publisher
var someArg = config["SOME_ARG"];

if (string.IsNullOrWhiteSpace(someArg))
{
    Console.WriteLine("[Receiver] Error: SOME_ARG environment variable is missing.");
    Environment.Exit(1);
}

Console.WriteLine($"[Receiver] Container started successfully.");
Console.WriteLine($"[Receiver] Processing task using for SOME_ARG variable: '{someArg}'");

// Execute your business logic here...

await Task.Delay(1000);

Console.WriteLine("[Receiver] Task complete. Container exiting.");