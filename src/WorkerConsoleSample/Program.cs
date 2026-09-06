
Console.WriteLine($"Args: {string.Join(",", args)}");

await Task.Delay(5000);

Console.WriteLine("WorkerConsoleSample - exit.");

return 0;