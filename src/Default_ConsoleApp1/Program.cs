
using System.Diagnostics;

Console.WriteLine(string.Join(",", args));

var random = new Random();
var delay = random.Next(1000, 10000);
await Task.Delay(delay);
Console.WriteLine($"Default_ConsoleApp1: {delay} - Hello, World! {DateTime.Now}");

var p = Process.GetCurrentProcess();
Console.WriteLine($"{nameof(p.MainModule)}:{p.MainModule}");
Console.WriteLine($"{nameof(p.Id)}:{p.Id}");
Console.WriteLine($"{nameof(p.BasePriority)}:{p.BasePriority}");
Console.WriteLine($"{nameof(p.Container)}:{p.Container}");
Console.WriteLine($"{nameof(p.Handle)}:{p.Handle}");
Console.WriteLine($"{nameof(p.ProcessName)}:{p.ProcessName}");
Console.WriteLine($"{nameof(p.WorkingSet64)}:{p.WorkingSet64}");

var r = random.Next(0, 2);

if(r==1)
{
    throw new Exception("Bad error");
}

Console.WriteLine("Exit");

