https://learn.microsoft.com/en-us/azure/container-instances/container-instances-overview


console app .NET Core
pass argument SQL connection string
serverless
docker container
cloud platform agnostic
runtime 50minutes
Trigger:	webhook

this console can have 
hundreds diffrent SQL connection strings
each time the webhook is triggerend need to pass or get it from somewhere

https://learn.microsoft.com/en-us/azure/container-instances/container-instances-container-groups
https://learn.microsoft.com/en-us/azure/container-instances/using-azure-container-registry-mi

VNet Integration and Private Endpoints.
https://permiso.io/

Sidecar Container

C# example HMAC Signatures in Headers webhook

docker container pass environment variables to another container

absolut task is how to pull secrets in there into child service
	payload can be done

webhook alternatives

Webhook
	HTTP POST request to receiver (child service)
	JSON data
	
push to all child services
	has Endpoint Reception

Environment.GetEnvironmentVariable

the secrets are dynamic, for every container instance

I have a docker container that is activated by push webhook, when the container starts needs to get the SQL connection string from somewhere secure, what are options, container is cloud platform agnostic

podman .NET
	docker in docker

qutable nees
	container name
	arguments
	environment Linux or Windows

## Kill container

startDate, containerId, maxRunTime

(DateTime.UtcNow-startDate) > maxRunTime

	// 3. Manually trigger the stop. THIS is when your StopTimeout clock starts ticking!
await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters
{
    WaitBeforeKillSeconds = 10 // This replaces StopTimeout dynamically on demand
});


ACR Roles
acrpull
Container Registry Repository Reader?
Azure Container Instances Contributor Role?

public record ContainerStart(DateTime StartDateTime, TimeSpan StopTimeout);


var containerRuns = new Dictionary<string, ContainerStart>(StringComparer.OrdinalIgnoreCase);

podman
https://developers.redhat.com/articles/2022/03/21/hello-podman-using-net#using_podman_from__net


curl -X POST https://publisherdockerdotnetpiperconsol.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"TenantId\":\"client-alpha\", \"EventType\":\"push\"}"
curl -X POST https://publisherdockerazuresdkpipercons.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"imageName\":\"development120260908161132/receiverstdinconsole2\"}"

curl -X POST https://publisherdockerazuresdkpipercons.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"imageName\":\"development120260908161132.azurecr.io/receiverstdinconsole2:latest\"}"
curl -X POST https://publisherdockerazuresdkpipercons.ashyground-0dd3034d.eastus.azurecontainerapps.io/webhook -H "Content-Type: application/json" -d "{\"imageName\":\"webapi20260903153932.azurecr.io/webfrontend:20260903063529\"}"


does not have authorization to perform action 'Microsoft.ContainerInstance/containerGroups/write' over scope '/subscriptions/xxx/resourceGroups/Development/providers/Microsoft.ContainerInstance/containerGroups/WebApi-env-20260903153212' or the scope 

	use, JSON ARM templates
	https://learn.microsoft.com/en-us/training/paths/deploy-manage-resource-manager-templates/


ar credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned); 
    var armClient = new ArmClient(credential);