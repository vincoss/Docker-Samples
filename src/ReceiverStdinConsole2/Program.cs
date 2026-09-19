using System.Text;

Console.WriteLine("ReceiverStdinConsole2 - Application starting up...");
Console.WriteLine($"Starting args: {string.Join(",", args)}");

var arr = await ReadWithTimeout();
if(arr == null || arr.Length <= 0)
{
    Console.WriteLine("No STDIN data.");
}
else
{
    string receivedChunk = Encoding.UTF8.GetString(arr);
    Console.WriteLine("STDIN data received.");
    Console.WriteLine(receivedChunk);
}

// Start your long-running application workflow.
await RunMainApplicationAsync();

Console.WriteLine("Exiting...");
return 0;

static async Task RunMainApplicationAsync()
{
    Console.WriteLine("Main application loop running.");
    var max = 60;
    var loops = 0;

    while(loops < max)
    {
        Console.WriteLine(loops);

        await Task.Delay(1000);
        loops++;
    }
}

static async Task<byte[]?> ReadWithTimeout(int timeoutMilliseconds = 500)
{
    if (Console.IsInputRedirected == false)
    {
        Console.WriteLine("Warning: No STDIN stream detected. Falling back to defaults.");

        return null;
    }

    Console.Error.WriteLine("Receiver container started. Waiting for STDIN data...");

    // Use task to timeout the wait for STDIN.
    var result = await Task.Run(() =>
    {
        Console.WriteLine("Begin read STDIN...");
        using Stream stdin = Console.OpenStandardInput();
        using var ms = new MemoryStream();
        stdin.CopyTo(ms);

        Console.Error.WriteLine("Host closed STDIN. Receiver exiting cleanly.");

        return ms.ToArray();

    }).WaitAsync(TimeSpan.FromMilliseconds(timeoutMilliseconds)); 

    return result;
}

static async Task<byte[]?> ReadWithTimeout2(int timeoutMilliseconds = 500)
{
    if (Console.IsInputRedirected == false)
    {
        Console.WriteLine("Warning: No STDIN stream detected. Falling back to defaults.");

        return null;
    }

    Console.Error.WriteLine("Receiver container started. Waiting for STDIN data...");

    var result = await Task.Run(() =>
    {
        Console.WriteLine("Begin read chunks...");

        // Open the raw STDIN stream
        using Stream stdin = Console.OpenStandardInput();
        byte[] buffer = new byte[4096];
        int bytesRead;

        // Process incoming data chunks as they arrive
        while ((bytesRead = stdin.Read(buffer, 0, buffer.Length)) > 0)
        {
            Console.WriteLine($"Reading data length: {bytesRead}");

            string receivedChunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Process the data (e.g., upper-case it)
            string processedData = receivedChunk.ToUpper();

            // Write the processed output back to STDOUT
            Console.WriteLine(processedData);
        }

        Console.Error.WriteLine("Host closed STDIN. Receiver exiting cleanly.");

        return buffer;

    }).WaitAsync(TimeSpan.FromMilliseconds(timeoutMilliseconds));

    return result;
}