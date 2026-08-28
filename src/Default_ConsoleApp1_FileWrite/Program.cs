Console.WriteLine("Hello, World!");

var basePath = AppContext.BaseDirectory;
var dataPath = Path.Combine(basePath, "appData");
var filePath = Path.Combine(dataPath, "test.txt");

Console.WriteLine(filePath);

File.WriteAllText(filePath, DateTime.UtcNow.ToString() + Environment.NewLine);
