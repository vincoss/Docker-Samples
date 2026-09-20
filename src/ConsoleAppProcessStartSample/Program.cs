using System.Diagnostics;


Console.WriteLine("Hello, World!");

var baseDirectory = AppContext.BaseDirectory;
var parentDirectory = Directory.GetParent(baseDirectory.TrimEnd(new[] { '\\', '/' }));
var completeAppPath = Path.Combine(parentDirectory.FullName, "Default_ConsoleApp1", "Default_ConsoleApp1.dll");

Console.WriteLine(baseDirectory);
Console.WriteLine(parentDirectory);
Console.WriteLine(completeAppPath);

bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

var arguments = $"-c \"dotnet {completeAppPath} r=a\"";

if(isWindows)
{
    arguments = $"/c \"dotnet {completeAppPath} r=a\"";
}

var startInfoWait = new ProcessStartInfo
{
    FileName = isWindows ? "cmd.exe" : "/bin/bash",
    Arguments = arguments,
    RedirectStandardOutput = true,   // Allows to read the output stream
    RedirectStandardError = true,    // Allows to read errors
    UseShellExecute = false,         // Required to redirect streams
    CreateNoWindow = true            // Runs invisibly in the background
};

 void LaunchProcessFireAndForget()
{
    // DO NOT use a 'using' block here. The GC will not collect the process 
    // while it is actively running and executing underlying native handles.
    var process = new Process();

    process.StartInfo = startInfoWait;

    // Hook up the async pipe streams
    process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine($"[Output]: {e.Data}"); };
    process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.WriteLine($"[Error]: {e.Data}"); };

    // Clean up unmanaged OS handles immediately on exit to prevent leaks
    process.EnableRaisingEvents = true;
    process.Exited += (sender, e) =>
    {
        Console.WriteLine($"Subprocess exited with code: {process.ExitCode}");
        process.Dispose();
    };

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
}

while (true)
{
    Console.WriteLine("Working...");

    LaunchProcessFireAndForget();

    Console.WriteLine("Waiting...");
    await Task.Delay(100);
}