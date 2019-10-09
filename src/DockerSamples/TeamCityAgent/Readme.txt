##Based on
https://hub.docker.com/r/jetbrains/teamcity-agent
https://hub.docker.com/_/microsoft-dotnet-framework
https://hub.docker.com/_/microsoft-dotnet-core-sdk/


## Get latest image
docker pull jetbrains/teamcity-minimal-agent:latest-windowsservercore

## Create volume
docker volume create --name=teamcityagent0001_data

## Build custom docker image
docker-compose build
docker-compose up --build

## Compose (scale agents)
docker-compose up -d
docker-compose up -d scale 2 # todo review

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down

## Error logs
docker logs --tail 50 --follow --timestamps TeamCityAgent0001

## Browse
http://localhost:8111/

## Get default password
docker cp jenkins:/var/jenkins_home/secrets/initialAdminPassword C:\Temp\Jenkins

------------------------------------------------- Create agent

1. create volume
docker volume create --name=teamcityagent0001_data

2. compose container
docker-compose up -d