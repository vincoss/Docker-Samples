
### Required roles for the controller container read and start container job
| Azure Role | Permission Required |
|---|---|---|
| Reader						| Read GetDefaultSubscription			|  
| Container Apps Jobs Operator	| Read and Start Container Apps Jobs	|


### Samples CURL
```

curl -X POST http://localhost:5000/webhook -H "Content-Type: application/json" -d "{\"JobData\":{},\"JobContainerName\":\"sample-container\",\"ImageName\":\"sample-image\"}"
curl -X POST http://localhost:5000/webhook -H "Content-Type: application/json" -d "{\"JobData\":{},\"JobContainerName\":\"sample-container\",\"ImageName\":\"sample-image\"}"

curl -X POST https://sampleazurecontainercontroller.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"JobData\":{},\"JobContainerName\":\"job1\"}"
curl -X POST https://sampleazurecontainercontroller.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"JobData\":{},\"JobContainerName\":\"job1\",\"ImageName\":\"sample-image\"}"

```

### Resources
https://github.com/Azure-Samples/container-apps-jobs/blob/main/README.md