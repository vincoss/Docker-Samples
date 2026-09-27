using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SampleRawHTTP
{
    public static class Samples
    {
        public static async Task SampleLinuxAsync()
        {
            // 1. Windows uses Named Pipes instead of file paths
            var pipeName = "docker_engine";

            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    // 2. Windows Named Pipes require NamedPipeClientStream
                    var pipeStream = new System.IO.Pipes.NamedPipeClientStream(".", pipeName, System.IO.Pipes.PipeDirection.InOut, System.IO.Pipes.PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };

            // 2. Create the Container
            var createResponse = await client.PostAsJsonAsync("/v1.43/containers/create?name=my-nginx", new
            {
                Image = "nginx:latest",
                ExposedPorts = new Dictionary<string, object> { { "80/tcp", new { } } },
                HostConfig = new { PortBindings = new Dictionary<string, object> { { "80/tcp", new[] { new { HostPort = "8080" } } } } }
            });

            var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
            string containerId = createResult.GetProperty("Id").GetString()!;

            // 3. Start the Container
            var startResponse = await client.PostAsync($"/v1.43/containers/{containerId}/start", null);

            if (startResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Container {containerId[..12]} started successfully on port 8080!");
            }
        }

        public static async Task SampleLinuxComleteAsync()
        {
            // 1. Establish connection via the Windows Docker Named Pipe
            var pipeName = "docker_engine";
            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };

            try
            {
                Console.WriteLine("Connecting to Docker Engine...");

                // 2. Define the container configuration (Using Nginx as an example)
                var containerConfig = new
                {
                    Image = "nginx:latest",
                    ExposedPorts = new Dictionary<string, object>
        {
            { "80/tcp", new { } }
        },
                    HostConfig = new
                    {
                        PortBindings = new Dictionary<string, object>
            {
                { "80/tcp", new[] { new { HostPort = "8080" } } }
            }
                    }
                };

                // 3. Request Docker to CREATE the container
                // Note: Make sure 'nginx:latest' is already pulled, or use a local image name instead!
                Console.WriteLine("Creating container...");
                var createResponse = await client.PostAsJsonAsync("/v1.43/containers/create?name=my-windows-nginx", containerConfig);

                if (!createResponse.IsSuccessStatusCode)
                {
                    string errorLog = await createResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create container: {errorLog}");
                    return;
                }

                var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
                string containerId = createResult.GetProperty("Id").GetString()!;
                Console.WriteLine($"Container created with ID: {containerId[..12]}");

                // 4. Request Docker to START the container
                Console.WriteLine("Starting container...");
                var startResponse = await client.PostAsync($"/v1.43/containers/{containerId}/start", null);

                if (startResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("🎉 Success! Container is running. Open http://localhost:8080 in your browser.");
                }
                else
                {
                    Console.WriteLine($"Failed to start container. Status code: {startResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static async Task SampleLinuxComletePullImagesAsync()
        {
            var pipeName = "docker_engine";
            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };

            try
            {
                // Configuration variables
                string imageName = "nginx";
                string imageTag = "latest";
                string fullImage = $"{imageName}:{imageTag}";
                string containerName = "my-custom-nginx";

                Console.WriteLine("Connecting to Docker Engine...");

                // 1. AUTOMATICALLY PULL IMAGE
                // The Docker API requires a POST request to /images/create to pull an image.
                Console.WriteLine($"Checking / pulling image '{fullImage}'...");
                var pullResponse = await client.PostAsync($"/v1.43/images/create?fromImage={imageName}&tag={imageTag}", null);

                if (!pullResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Warning: Failed to pull image via API. Will try to proceed with local cache.");
                }
                else
                {
                    // The Docker API streams the pull progress. We read it to ensure the pull finishes before moving on.
                    var progressStream = await pullResponse.Content.ReadAsStreamAsync();
                    using var reader = new System.IO.StreamReader(progressStream);
                    while (await reader.ReadLineAsync() != null) { /* Wait for stream to complete */ }
                    Console.WriteLine("Image ready!");
                }

                // 2. DEFINE ENVIRONMENT VARIABLES AND VOLUME MOUNTS
                var containerConfig = new
                {
                    Image = fullImage,

                    // Environment Variables (Array of "KEY=VALUE" strings)
                    Env = new[]
                    {
            "MY_ENV_VAR=HelloFromCSharp",
            "ANOTHER_SETTING=12345"
        },

                    ExposedPorts = new Dictionary<string, object> { { "80/tcp", new { } } },

                    HostConfig = new
                    {
                        PortBindings = new Dictionary<string, object>
            {
                { "80/tcp", new[] { new { HostPort = "8080" } } }
            },

                        // Volume Mounts (Binds)
                        // Format: "C:\\host\\path:/container/path:rw"
                        // Note: Use forward slashes inside the container path, and double backslashes for Windows host paths.
                        Binds = new[]
                        {
                "C:\\temp\\nginx_logs:/var/log/nginx:rw"
            }
                    }
                };

                // 3. CREATE THE CONTAINER
                Console.WriteLine($"Creating container '{containerName}'...");
                var createResponse = await client.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);

                if (!createResponse.IsSuccessStatusCode)
                {
                    string errorLog = await createResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create container: {errorLog}");
                    return;
                }

                var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
                string containerId = createResult.GetProperty("Id").GetString()!;
                Console.WriteLine($"Container created! ID: {containerId[..12]}");

                // 4. START THE CONTAINER
                Console.WriteLine("Starting container...");
                var startResponse = await client.PostAsync($"/v1.43/containers/{containerId}/start", null);

                if (startResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("🎉 Success! Your container is running with custom environment variables and a volume mount.");
                    Console.WriteLine("🔗 Web Access: http://localhost:8080");
                    Console.WriteLine("📁 Local Host Volume Created At: C:\\temp\\nginx_logs");
                }
                else
                {
                    Console.WriteLine($"Failed to start container. Status: {startResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static async Task SamleLinuxWriteToConsole()
        {
            var pipeName = "docker_engine";
            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            using var client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };
            // Set timeout to Infinite so the connection doesn't drop while reading logs forever
            client.Timeout = Timeout.InfiniteTimeSpan;

            try
            {
                string imageName = "nginx";
                string imageTag = "latest";
                string fullImage = $"{imageName}:{imageTag}";
                string containerName = "my-custom-nginx";

                Console.WriteLine("Connecting to Docker Engine...");

                // 1. AUTOMATICALLY PULL IMAGE
                Console.WriteLine($"Checking / pulling image '{fullImage}'...");
                var pullResponse = await client.PostAsync($"/v1.43/images/create?fromImage={imageName}&tag={imageTag}", null);
                if (pullResponse.IsSuccessStatusCode)
                {
                    var progressStream = await pullResponse.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(progressStream);
                    while (await reader.ReadLineAsync() != null) { }
                }

                // 2. CONTAINER CONFIGURATION
                var containerConfig = new
                {
                    Image = fullImage,
                    Env = new[] { "MY_ENV_VAR=HelloFromCSharp" },
                    ExposedPorts = new Dictionary<string, object> { { "80/tcp", new { } } },
                    HostConfig = new
                    {
                        PortBindings = new Dictionary<string, object> { { "80/tcp", new[] { new { HostPort = "8080" } } } }
                    }
                };

                // 3. CREATE CONTAINER WITH CONFLICT HANDLING
                Console.WriteLine($"Creating container '{containerName}'...");
                var createResponse = await client.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);

                if (createResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"⚠️ Container name '{containerName}' is already in use. Cleaning it up...");
                    await client.PostAsync($"/v1.43/containers/{containerName}/stop?t=5", null);
                    await client.DeleteAsync($"/v1.43/containers/{containerName}");
                    createResponse = await client.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);
                }

                if (!createResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create container: {await createResponse.Content.ReadAsStringAsync()}");
                    return;
                }

                var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
                string containerId = createResult.GetProperty("Id").GetString()!;
                Console.WriteLine($"Container created! ID: {containerId[..12]}");

                // 4. START THE CONTAINER
                Console.WriteLine("Starting container...");
                var startResponse = await client.PostAsync($"/v1.43/containers/{containerId}/start", null);

                if (!startResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to start container. Status: {startResponse.StatusCode}");
                    return;
                }

                Console.WriteLine("🎉 Success! Your container is running.");
                Console.WriteLine("🔗 Web Access: http://localhost:8080");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("📋 STARTING LIVE LOG STREAM (Press Ctrl+C inside terminal to exit)");
                Console.WriteLine("----------------------------------------------------------------");

                // 5. STREAM RUNNING LOGS
                // Parameters requested: follow=true (keep connection open), stdout=true, stderr=true
                var logUrl = $"/v1.43/containers/{containerId}/logs?follow=true&stdout=true&stderr=true";

                // Use HttpCompletionOption.ResponseHeadersRead to stream data instantly instead of buffering it
                using var logResponse = await client.GetAsync(logUrl, HttpCompletionOption.ResponseHeadersRead);
                using var stream = await logResponse.Content.ReadAsStreamAsync();

                // Docker multiplexes logs (attaches an 8-byte header to distinguish stdout vs stderr).
                // This buffer parses the stream correctly.
                byte[] header = new byte[8];
                while (true)
                {
                    // Read the 8-byte Docker stream header
                    int bytesRead = await stream.ReadAsync(header, 0, 8);
                    if (bytesRead < 8) break; // Stream closed

                    // Big-endian payload size calculation from bytes 4-7 of the header
                    int payloadSize = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | header[7];

                    byte[] payload = new byte[payloadSize];
                    int totalPayloadBytesRead = 0;

                    while (totalPayloadBytesRead < payloadSize)
                    {
                        int read = await stream.ReadAsync(payload, totalPayloadBytesRead, payloadSize - totalPayloadBytesRead);
                        if (read == 0) break;
                        totalPayloadBytesRead += read;
                    }

                    string logMessage = Encoding.UTF8.GetString(payload);
                    Console.Write(logMessage); // Nginx logs already include trailing newlines (\n)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static HttpClient? _client;
        private static string? _containerId;
        private static readonly CancellationTokenSource _cts = new();

        public static async Task SamleLinuxWriteToConsoleHealthCleandDeleteContainers()
        {
            var pipeName = "docker_engine";
            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            _client = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };
            _client.Timeout = Timeout.InfiniteTimeSpan;

            // 1. REGISTER AUTOMATIC CLEANUP HANDLERS
            // This stops and deletes the container when the console application exits or Ctrl+C is pressed.
            AppDomain.CurrentDomain.ProcessExit += (s, e) => ExecuteCleanup();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("\nStopping application...");
                e.Cancel = true; // Prevents immediate hard crash so cleanup can run safely
                _cts.Cancel();    // Cancels the log streaming loop
            };

            try
            {
                string imageName = "nginx";
                string imageTag = "latest";
                string fullImage = $"{imageName}:{imageTag}";
                string containerName = "my-custom-nginx";

                Console.WriteLine("Connecting to Docker Engine...");

                // 2. AUTOMATICALLY PULL IMAGE
                Console.WriteLine($"Checking / pulling image '{fullImage}'...");
                var pullResponse = await _client.PostAsync($"/v1.43/images/create?fromImage={imageName}&tag={imageTag}", null);
                if (pullResponse.IsSuccessStatusCode)
                {
                    var progressStream = await pullResponse.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(progressStream);
                    while (await reader.ReadLineAsync() != null) { }
                }

                // 3. CONTAINER CONFIGURATION
                var containerConfig = new
                {
                    Image = fullImage,
                    Env = new[] { "MY_ENV_VAR=HelloFromCSharp" },
                    ExposedPorts = new Dictionary<string, object> { { "80/tcp", new { } } },
                    HostConfig = new
                    {
                        PortBindings = new Dictionary<string, object> { { "80/tcp", new[] { new { HostPort = "8080" } } } }
                    }
                };

                // 4. CREATE CONTAINER WITH CONFLICT HANDLING
                Console.WriteLine($"Creating container '{containerName}'...");
                var createResponse = await _client.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);

                if (createResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"⚠️ Container name '{containerName}' is already in use. Cleaning up stale instance...");
                    await _client.PostAsync($"/v1.43/containers/{containerName}/stop?t=5", null);
                    await _client.DeleteAsync($"/v1.43/containers/{containerName}");
                    createResponse = await _client.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);
                }

                if (!createResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create container: {await createResponse.Content.ReadAsStringAsync()}");
                    return;
                }

                var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
                _containerId = createResult.GetProperty("Id").GetString()!;
                Console.WriteLine($"Container created! ID: {_containerId[..12]}");

                // 5. START THE CONTAINER
                Console.WriteLine("Starting container...");
                var startResponse = await _client.PostAsync($"/v1.43/containers/{_containerId}/start", null);

                if (!startResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to start container. Status: {startResponse.StatusCode}");
                    return;
                }

                Console.WriteLine("🎉 Success! Your container is running.");
                Console.WriteLine("🔗 Web Access: http://localhost:8080");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("📋 LOG STREAM ACTIVE (Press Ctrl+C to close app and remove container)");
                Console.WriteLine("----------------------------------------------------------------");

                // 6. STREAM RUNNING LOGS
                var logUrl = $"/v1.43/containers/{_containerId}/logs?follow=true&stdout=true&stderr=true";
                using var logResponse = await _client.GetAsync(logUrl, HttpCompletionOption.ResponseHeadersRead, _cts.Token);
                using var stream = await logResponse.Content.ReadAsStreamAsync(_cts.Token);

                byte[] header = new byte[8];
                while (!_cts.Token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(header, 0, 8, _cts.Token);
                    if (bytesRead < 8) break;

                    int payloadSize = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | header[7];
                    byte[] payload = new byte[payloadSize];
                    int totalPayloadBytesRead = 0;

                    while (totalPayloadBytesRead < payloadSize)
                    {
                        int read = await stream.ReadAsync(payload, totalPayloadBytesRead, payloadSize - totalPayloadBytesRead, _cts.Token);
                        if (read == 0) break;
                        totalPayloadBytesRead += read;
                    }

                    string logMessage = Encoding.UTF8.GetString(payload);
                    Console.Write(logMessage);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when Ctrl+C fires
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // 7. CLEANUP LOGIC EXECUTED ON EXIT
        private static void ExecuteCleanup()
        {
            if (_client == null || string.IsNullOrEmpty(_containerId)) return;

            Console.WriteLine("\n🧹 Running automatic cleanup...");
            try
            {
                // Stop the container synchronously since async execution isn't guaranteed reliable during process exit
                Console.WriteLine("Stopping container...");
                var stopTask = _client.PostAsync($"/v1.43/containers/{_containerId}/stop?t=2", null);
                stopTask.Wait(TimeSpan.FromSeconds(5));

                Console.WriteLine("Deleting container...");
                var deleteTask = _client.DeleteAsync($"/v1.43/containers/{_containerId}");
                deleteTask.Wait(TimeSpan.FromSeconds(5));

                Console.WriteLine("🗑️ Cleanup complete. Environment is clean!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up container automatically: {ex.Message}");
            }
        }



        private static HttpClient? _clientWindows;
        private static string? _containerIdWindows;
        private static readonly CancellationTokenSource _ctsWindows = new();

        public static async Task SampleWindowsAsync()
        {
            // 1. DOCKER WINDOWS NAMED PIPE CLIENT CONFIGURATION
            var pipeName = "docker_engine";
            var connectionHandler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    var pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    await pipeStream.ConnectAsync(token);
                    return pipeStream;
                }
            };

            _clientWindows = new HttpClient(connectionHandler) { BaseAddress = new Uri("http://localhost") };
            _clientWindows.Timeout = Timeout.InfiniteTimeSpan;

            // Register automatic cleanup handlers on close/interruption
            AppDomain.CurrentDomain.ProcessExit += (s, e) => ExecuteCleanupWindows();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("\nStopping application...");
                e.Cancel = true;
                _cts.Cancel();
            };

            try
            {
                // 2. WINDOWS HELLO-WORLD CONFIGURATION
                // Note: Your Docker Desktop environment must be explicitly switched to "Windows Containers" mode for this to run!
                string imageName = "hello-world";
                string imageTag = "nanoserver-ltsc2022";
                string fullImage = $"{imageName}:{imageTag}";
                string containerName = "my-windows-hello-world";

                Console.WriteLine("Connecting to Docker Engine...");

                // 3. AUTOMATICALLY PULL THE WINDOWS IMAGE
                Console.WriteLine($"Checking / pulling Windows image '{fullImage}'...");
                var pullResponse = await _clientWindows.PostAsync($"/v1.43/images/create?fromImage={imageName}&tag={imageTag}", null);
                if (pullResponse.IsSuccessStatusCode)
                {
                    var progressStream = await pullResponse.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(progressStream);
                    while (await reader.ReadLineAsync() != null) { }
                    Console.WriteLine("Image ready!");
                }

                // 4. CONTAINER CONFIGURATION (Simplified without port bindings since hello-world isn't a web service)
                var containerConfig = new
                {
                    Image = fullImage,
                    Env = new[] { "HELLO_TARGET=WindowsContainers" }
                };

                // 5. CREATE CONTAINER WITH CONFLICT HANDLING
                Console.WriteLine($"Creating Windows container '{containerName}'...");
                var createResponse = await _clientWindows.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);

                if (createResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"⚠️ Container name '{containerName}' is already in use. Cleaning up stale instance...");
                    await _clientWindows.PostAsync($"/v1.43/containers/{containerName}/stop?t=2", null);
                    await _clientWindows.DeleteAsync($"/v1.43/containers/{containerName}");
                    createResponse = await _clientWindows.PostAsJsonAsync($"/v1.43/containers/create?name={containerName}", containerConfig);
                }

                if (!createResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create container: {await createResponse.Content.ReadAsStringAsync()}");
                    return;
                }

                var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();

                // 1. Assign to your Windows container variable
                _containerIdWindows = createResult.GetProperty("Id").GetString()!;

                // 2. Fix the print line to reference _containerIdWindows instead of _containerId
                Console.WriteLine($"Container created! ID: {_containerIdWindows[..12]}");

                // 6. START THE WINDOWS CONTAINER
                Console.WriteLine("Starting container...");
                var startResponse = await _clientWindows.PostAsync($"/v1.43/containers/{_containerId}/start", null);

                if (!startResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to start container. Status: {startResponse.StatusCode}");
                    return;
                }

                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("📋 LOG STREAM ACTIVE (Reading output from hello-world process)");
                Console.WriteLine("----------------------------------------------------------------");

                // 7. STREAM REPLAYED RUNNING LOGS
                var logUrl = $"/v1.43/containers/{_containerId}/logs?follow=true&stdout=true&stderr=true";
                using var logResponse = await _clientWindows.GetAsync(logUrl, HttpCompletionOption.ResponseHeadersRead, _cts.Token);
                using var stream = await logResponse.Content.ReadAsStreamAsync(_cts.Token);

                byte[] header = new byte[8];
                while (!_cts.Token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(header, 0, 8, _cts.Token);
                    if (bytesRead < 8) break; // End of log stream reached

                    int payloadSize = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | header[7];
                    byte[] payload = new byte[payloadSize];
                    int totalPayloadBytesRead = 0;

                    while (totalPayloadBytesRead < payloadSize)
                    {
                        int read = await stream.ReadAsync(payload, totalPayloadBytesRead, payloadSize - totalPayloadBytesRead, _cts.Token);
                        if (read == 0) break;
                        totalPayloadBytesRead += read;
                    }

                    string logMessage = Encoding.UTF8.GetString(payload);
                    Console.Write(logMessage);
                }
            }
            catch (OperationCanceledException)
            {
                // Fired gracefully upon exit
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ExecuteCleanupWindows()
        {
            if (_clientWindows == null || string.IsNullOrEmpty(_containerId)) return;

            Console.WriteLine("\n\n🧹 Running automatic cleanup...");
            try
            {
                // Windows containers can take a moment to stop cleanly
                var stopTask = _clientWindows.PostAsync($"/v1.43/containers/{_containerId}/stop?t=2", null);
                stopTask.Wait(TimeSpan.FromSeconds(5));

                var deleteTask = _clientWindows.DeleteAsync($"/v1.43/containers/{_containerId}");
                deleteTask.Wait(TimeSpan.FromSeconds(5));

                Console.WriteLine("🗑️ Cleanup complete. Windows host environment is clear!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up container automatically: {ex.Message}");
            }
        }
    }
}