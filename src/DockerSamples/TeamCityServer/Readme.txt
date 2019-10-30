##Based on
https://hub.docker.com/r/jetbrains/teamcity-server
https://docs.docker.com/compose/compose-file/
https://www.ntweekly.com/2018/04/02/copy-data-files-windows-container/

## Get latest image
docker pull jetbrains/teamcity-server:latest-nanoserver

## Create volume
docker volume create --name=teamcityserver_data
docker volume create --name=teamcityserver_logs

## Create network
docker network create --driver nat isolated_nw

## Build custom docker image
docker-compose build
docker-compose up --build

## Compose & destroy
docker-compose up -d

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down

## Error logs
docker logs --tail 50 --follow --timestamps TeamCityServer

## Open PowerShell to running container
docker exec -it containerName powershell

## Browse
http://localhost:8111/
http://TeamCityServer:8111/

## Get default password (for windows container need to stop it)
docker cp TeamCityServer:C:\TeamCity\logs C:\Temp\TeamCityServer\logs

#Location on host machine (windows)
C:\ProgramData\Docker\volumes

------------------------------------------------- Create teamcity server

1. create volume
docker volume create --name=teamcityserver_data
docker volume create --name=teamcityserver_logs

2. create network (NOTE: Uses switch created from Hyper-v: b3f21147276e		External-Primary-Virtual-Switch		transparent		local)
docker network create --driver transparent isolated_nw

3. run container & destroy
docker-compose up -d