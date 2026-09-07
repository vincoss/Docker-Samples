using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Docker.TesterWebApi.Controllers
{
    [ApiController]
    [Route("api/diagnostics")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IOptionsSnapshot<AppSettings> _options;

        public DiagnosticsController(IOptionsSnapshot<AppSettings> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = new
            {
                Environment.HasShutdownStarted,
                Environment.MachineName,
                Environment.ExitCode,
                Environment.CurrentManagedThreadId,
                Environment.CurrentDirectory,
                Environment.CommandLine,
                Environment.NewLine,
                Environment.OSVersion,
                Environment.ProcessorCount,
                Environment.Is64BitOperatingSystem,
                Environment.StackTrace,
                Environment.SystemPageSize,
                Environment.TickCount,
                Environment.TickCount64,
                Environment.UserDomainName,
                Environment.UserInteractive,
                Environment.UserName,
                Environment.Version,
                Environment.WorkingSet,
                Environment.SystemDirectory,
                Environment.Is64BitProcess,
                GetCommandLineArgs = Environment.GetCommandLineArgs(),
                GetEnvironmentVariables = Environment.GetEnvironmentVariables(),
                GetLogicalDrives = Environment.GetLogicalDrives()
            };

            var json = System.Text.Json.JsonSerializer.Serialize(result);

            return Ok(result);
        }

        [HttpGet]
        [Route("getAppDataFileList")]
        public IActionResult GetAppDataFileList()
        {
            var path = _options.Value.DataPath;

            var result = new
            {
                DataPath = path,
                Files = Directory.Exists(path) ? Directory.GetFiles(path) : new[] { "Does not exits" }
            };

            return Ok(result);
        }


        [HttpGet]
        [Route("getNetworkInfo")]
        public IActionResult GetNetworkInfo()
        {
            var ipV4s = NetworkInterface.GetAllNetworkInterfaces()
                        .Select(i => i.GetIPProperties().UnicastAddresses)
                        .SelectMany(u => u)
                        .Select(i => i.Address.ToString());

                return Ok(ipV4s);
        }

        [HttpGet]
        [Route("writeTestFile")]
        public IActionResult WriteTestFile()
        {
            var path = Path.Combine(_options.Value.DataPath, $"{nameof(WriteTestFile)}.txt");

            System.IO.File.WriteAllText(path, DateTime.Now.ToString());

            return Ok(path);
        }
    }
}