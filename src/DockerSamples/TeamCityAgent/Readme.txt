##Based on
https://hub.docker.com/r/jetbrains/teamcity-server/


## Get latest image
docker pull jetbrains/teamcity-minimal-agent:latest-nanoserver

## Create volume
docker volume create --name=teamcityagent0001_data

## Build custom docker image
docker-compose build
docker-compose up --build

## Compose
docker-compose up -d

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down

## Error logs
docker logs --tail 50 --follow --timestamps jenkins

## Browse
http://localhost:8111/

## Get default password
docker cp jenkins:/var/jenkins_home/secrets/initialAdminPassword C:\Temp\Jenkins