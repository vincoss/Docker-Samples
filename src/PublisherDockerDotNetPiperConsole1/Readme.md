
### Info
Use with ReceiverStdinConsole2 project.

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