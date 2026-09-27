using SampleDotNetTestcontainersContainerController;

Console.WriteLine("Hello, World!");

await new DotNetTestcontainersSample().RunWindowsAsync();

Console.WriteLine("Conainer started...");
Console.WriteLine("Exit...");