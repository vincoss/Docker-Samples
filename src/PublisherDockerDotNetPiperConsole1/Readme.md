
### Info
Use with ReceiverStdinConsole2 project.

### Issues - Connection failed.
```
Connection failed.  ---> System.Net.Sockets.SocketException (99): Cannot assign requested address

start container with volumne for linux
-v /var/run/docker.sock:/var/run/docker.sock

start container with volumne for windows
-v \\.\pipe\docker_engine:\\.\pipe\docker_engine

```

### Curl
```
curl -X POST http://localhost:5000/webhook \
-H "Content-Type: application/json" \
-d '{"TenantId": "client-alpha", "EventType": "push"}'
```

### Cmd
```
curl -X POST http://localhost:5000/webhook -H "Content-Type: application/json" -d "{\"TenantId\":\"client-alpha\", \"EventType\":\"push\"}"
```

### Resources
https://github.com/dotnet/Docker.DotNet
https://github.com/testcontainers/Docker.DotNet