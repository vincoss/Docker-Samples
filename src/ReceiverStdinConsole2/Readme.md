

### logs
```
ContainerAppConsoleLogs_CL
| where ContainerJobName_s == "job1"
| project TimeGenerated, Log_s, ExecutionName_s
| order by TimeGenerated desc
```