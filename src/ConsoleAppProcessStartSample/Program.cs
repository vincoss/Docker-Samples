using System.Diagnostics;
using System.Text;

Console.WriteLine("Hello, World!");

var baseDirectory = AppContext.BaseDirectory;
var parentDirectory = Directory.GetParent(baseDirectory.TrimEnd(new[] { '\\', '/' }));
var completeAppPath = Path.Combine(parentDirectory.FullName, "Default_ConsoleApp1", "Default_ConsoleApp1.exe");

Console.WriteLine(baseDirectory);
Console.WriteLine(parentDirectory);
Console.WriteLine(completeAppPath);

var startInfo = new ProcessStartInfo
{
    FileName = completeAppPath,
    Arguments = "/all",
    RedirectStandardOutput = true,   // Allows C# to read the output stream
    RedirectStandardError = true,    // Allows C# to read errors
    UseShellExecute = false,         // Required to redirect streams
    CreateNoWindow = true            // Runs invisibly in the background
};

while(true)
{
    using (Process process = Process.Start(startInfo))
    {
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        // Attach event handlers to read data dynamically
        process.OutputDataReceived += (sender, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

        process.Start();

        // Start the asynchronous read
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for the process to exit safely
        process.WaitForExit();

        string output = outputBuilder.ToString();
        string error = errorBuilder.ToString();

        Console.WriteLine("Output:");
        Console.WriteLine(output);
        Console.WriteLine(error);
    }

    await Task.Delay(5000);
}