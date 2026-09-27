using SampleRawHTTP;
using SampleRawHTTP.Services;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

Console.WriteLine("SampleRawHTTP - Hello, World!");

await AppContainerServiceTester.RunWindowsAsync();
