using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace SampleRawHTTP.Services
{
    public static class AppContainerServiceTester
    {
        public static async Task RunLinuxAsync()
        {
            var nginxJob = new ContainerJobDto
            {
                Name = "production-web-proxy",
                ImageName = "nginx:latest",

                // Command line arguments passed directly to the entrypoint process
                Args = new List<string>
            {
                "-g",
                "daemon off;"
            }
            };

            // 1. Overriding the default container startup binary path (Optional)
            nginxJob.Commands.Add("/usr/sbin/nginx");

            // 2. Injecting standard Environment Variables
            nginxJob.Envs.Add(new ContainerJobDto.EnvironmentVariable
            {
                Name = "NGINX_HOST",
                Value = "example.com"
            });

            nginxJob.Envs.Add(new ContainerJobDto.EnvironmentVariable
            {
                Name = "NGINX_PORT",
                Value = "80"
            });

            // 3. Injecting a tracked reference secret
            nginxJob.Envs.Add(new ContainerJobDto.EnvironmentVariable
            {
                Name = "SSL_DECRYPTION_KEY",
                SecretRef = "vault://production/nginx/ssl-key"
            });

            var service = new AppContainerService();
            var cts = new CancellationTokenSource();

            await service.StartAsync(nginxJob, cts.Token);
        }

        public static async Task RunWindowsAsync()
        {
            // Define a native Windows hello-world container job
            var windowsJob = new ContainerJobDto
            {
                Name = "production-windows-test",

                // Critical: Use the nanoserver build for native Windows containers
                ImageName = "hello-world:nanoserver",

                // hello-world doesn't require extra args or commands, 
                // but we can pass an env variable to test ingestion
                Args = new List<string>()
            };

            windowsJob.Envs.Add(new ContainerJobDto.EnvironmentVariable
            {
                Name = "TARGET_PLATFORM",
                Value = "WindowsContainers"
            });

            var service = new AppContainerService();
            var cts = new CancellationTokenSource();

            await service.StartAsync(windowsJob, cts.Token);
        }
    }


    public class ContainerJobDto
    {
        public required string Name { get; set; }

        public required string ImageName { get; set; }

        public IList<string> Args { get; set; } = new List<string>();

        public IList<string> Commands { get; } = new List<string>();

        public IList<EnvironmentVariable> Envs { get; } = new List<EnvironmentVariable>();

        public class EnvironmentVariable
        {
            public required string Name { get; set; }

            public string? Value { get; set; }

            public string? SecretRef { get; set; }
        }
    }

    public interface IAppContainerService
    {
        Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken);
        Task<long> WaitForExitAsync(string containerId, CancellationToken cancellationToken);
    }

    public class AppContainerService : IAppContainerService
    {
        private readonly string _socketOrPipePath;
        private readonly bool _isWindows;

        public AppContainerService(string? customPath = null)
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            // Auto-detect host operating system to assign the correct default socket communication stream
            if (_isWindows)
            {
                _socketOrPipePath = string.IsNullOrWhiteSpace(customPath) ? "docker_engine" : customPath;
            }
            else
            {
                _socketOrPipePath = string.IsNullOrWhiteSpace(customPath) ? "/var/run/docker.sock" : customPath;
            }
        }

        public async Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(job);

            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    if (_isWindows)
                    {
                        var pipeStream = new NamedPipeClientStream(".", _socketOrPipePath, PipeDirection.InOut, PipeOptions.Asynchronous);
                        await pipeStream.ConnectAsync(token);
                        return pipeStream;
                    }
                    else
                    {
                        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                        try
                        {
                            await socket.ConnectAsync(new UnixDomainSocketEndPoint(_socketOrPipePath), token);
                            return new NetworkStream(socket, ownsSocket: true);
                        }
                        catch
                        {
                            socket.Dispose();
                            throw;
                        }
                    }
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };

            try
            {
                var envStrings = job.Envs?
                    .Select(e => $"{e.Name}={e.Value ?? e.SecretRef ?? string.Empty}")
                    .ToArray() ?? Array.Empty<string>();

                var containerConfig = new
                {
                    Image = job.ImageName,
                    Entrypoint = job.Commands != null && job.Commands.Count > 0 ? job.Commands.ToArray() : null,
                    Cmd = job.Args != null && job.Args.Count > 0 ? job.Args.ToArray() : null,
                    Env = envStrings,

                    HostConfig = new
                    {
                        AutoRemove = true,

                         // Forces the container to wrap inside a safe lightweight virtual machine 
                         // if your Windows OS version doesn't exactly match the image version
                        Isolation = "hyperv"
                    }
                };

                string createUrl = $"/v1.43/containers/create?name={Uri.EscapeDataString(job.Name)}";
                using var createResponse = await client.PostAsJsonAsync(createUrl, containerConfig, cancellationToken);

                if (createResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"⚠️ Container '{job.Name}' already exists. Recreating...");
                    await client.PostAsync($"/v1.43/containers/{Uri.EscapeDataString(job.Name)}/stop?t=2", null, cancellationToken);
                    await client.DeleteAsync($"/v1.43/containers/{Uri.EscapeDataString(job.Name)}", cancellationToken);

                    using var retryResponse = await client.PostAsJsonAsync(createUrl, containerConfig, cancellationToken);
                    await ProcessCreationAndStartAsync(client, retryResponse, job.Name, cancellationToken);
                    return;
                }

                await ProcessCreationAndStartAsync(client, createResponse, job.Name, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running container job '{job.Name}': {ex.Message}");
                throw;
            }
        }

        private static async Task ProcessCreationAndStartAsync(HttpClient client, HttpResponseMessage createResponse, string containerName, CancellationToken cancellationToken)
        {
            if (!createResponse.IsSuccessStatusCode)
            {
                string errorDetails = await createResponse.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"❌ Error: Failed to create container. HTTP Status: {(int)createResponse.StatusCode} ({createResponse.StatusCode}). Details: {errorDetails}");
                return;
            }

            var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            string containerId = createResult.GetProperty("Id").GetString()!;

            using var startResponse = await client.PostAsync($"/v1.43/containers/{containerId}/start", null, cancellationToken);

            Console.WriteLine($"Status Code: {(int)startResponse.StatusCode}");

            if (startResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Container ID: {containerId}");
            }
            else
            {
                string startError = await startResponse.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"❌ Error: Failed to start container '{containerName}'. Details: {startError}");
            }
        }


        /// <summary>
        /// Asynchronously blocks until the container completes execution, returning its final exit status code.
        /// </summary>
        public async Task<long> WaitForExitAsync(string containerId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new ArgumentException("Container ID cannot be null or empty.", nameof(containerId));

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                    try
                    {
                        await socket.ConnectAsync(new UnixDomainSocketEndPoint(_socketOrPipePath), token);
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };
            client.Timeout = Timeout.InfiniteTimeSpan;

            try
            {
                string waitUrl = $"/v1.43/containers/{containerId}/wait";

                // FIX 1: Create an explicit HttpRequestMessage to use HttpCompletionOption with a POST
                using var request = new HttpRequestMessage(HttpMethod.Post, waitUrl);

                // Use SendAsync instead of PostAsync to safely supply ResponseHeadersRead
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    // FIX 2: Pass token correctly based on standard target frameworks
                    string errorDetails = await response.Content.ReadAsStringAsync(linkedCts.Token);
                    throw new InvalidOperationException($"Failed to register waiter for container {containerId[..12]}. API Error: {errorDetails}");
                }

                // Stream down the JSON body chunk safely using the linked token
                using var responseStream = await response.Content.ReadAsStreamAsync(linkedCts.Token);
                var result = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream, cancellationToken: linkedCts.Token);

                long exitCode = result.GetProperty("StatusCode").GetInt64();

                Console.WriteLine($"Container {containerId[..12]} finished executing. Exit Code: {exitCode}");
                return exitCode;
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                string timeoutErrorMessage = $"❌ Error: Container job {containerId[..12]} exceeded the 30-minute time cap and was aborted.";
                Console.WriteLine(timeoutErrorMessage);

                try
                {
                    await client.PostAsync($"/v1.43/containers/{containerId}/kill", null, CancellationToken.None);
                    Console.WriteLine($"Successfully sent kill signal to runaway container {containerId[..12]}.");
                }
                catch (Exception killEx)
                {
                    Console.WriteLine($"Failed to kill runaway container: {killEx.Message}");
                }

                throw new TimeoutException(timeoutErrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while waiting for container {containerId[..12]} to finish: {ex.Message}");
                throw;
            }
        }
    }
}
