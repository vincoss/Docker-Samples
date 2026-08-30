Console.WriteLine("Hello, World!");

var basePath = AppContext.BaseDirectory;
var dataPath = Path.Combine(basePath, "appdata");
var filePath = Path.Combine(dataPath, "test.txt");

Console.WriteLine(filePath);

if (Directory.Exists(dataPath) == false) Directory.CreateDirectory(dataPath);

File.WriteAllText(filePath, DateTime.UtcNow.ToString() + Environment.NewLine);