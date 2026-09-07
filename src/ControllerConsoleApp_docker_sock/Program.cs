using System.Diagnostics;

Console.WriteLine("ControllerConsoleApp_docker_sock");

var startInfo = new ProcessStartInfo
{
    FileName = "docker",
    Arguments = "run --rm workerconsolesample_windows --environment=development",
    RedirectStandardOutput = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

for (int i = 0; i < 10; i++)
{
    using var process = Process.Start(startInfo);
    process.Start();
    Console.WriteLine(process.Id);

    await Task.Delay(10000);
}

Console.WriteLine("ControllerConsoleApp_docker_sock - exit");