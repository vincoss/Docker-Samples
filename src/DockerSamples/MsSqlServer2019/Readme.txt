##Based on
https://hub.docker.com/r/jetbrains/teamcity-server
https://docs.docker.com/compose/compose-file/
https://www.ntweekly.com/2018/04/02/copy-data-files-windows-container/

## Get latest image
docker pull mcr.microsoft.com/mssql/server:2019-CTP3.2-ubuntu

## Create volume
docker volume create --name=sqlserverexpress2019_data

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
docker logs --tail 50 --follow --timestamps sqlserverexpress2019

## Open PowerShell to running container
docker exec -it containerName powershell

#Location on host machine (windows)
C:\ProgramData\Docker\volumes

------------------------------------------------- Create teamcity server

1. create volume
docker volume create --name=sqlserverexpress2019_data

2. create network (NOTE: Uses switch created from Hyper-v: b3f21147276e		External-Primary-Virtual-Switch		transparent		local)
docker network create --driver transparent isolated_nw

3. run container & destroy
docker-compose up -d