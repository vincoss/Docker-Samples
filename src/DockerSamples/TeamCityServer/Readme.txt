##Based on
https://hub.docker.com/r/jetbrains/teamcity-server/
https://www.ntweekly.com/2018/04/02/copy-data-files-windows-container/

## Get latest image
docker pull jetbrains/teamcity-server:latest-nanoserver

## Create volume
docker volume create --name=teamcityserver_data
docker volume create --name=teamcityserver_logs

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
docker logs --tail 50 --follow --timestamps TeamCityServer

## Browse
http://localhost:8111/
http://TeamCityServer:8111/

## Get default password (for windows container need to stop it)
docker cp TeamCityServer:C:\TeamCity\logs C:\Temp\TeamCityServer\logs

#Location on host machine (windows)
C:\ProgramData\Docker\volumes