##Based on
https://hub.docker.com/r/jetbrains/teamcity-server
https://docs.docker.com/compose/compose-file/

## Get latest image
docker pull jetbrains/teamcity-server

## Create volume
docker volume create --name=teamcityserver

## Create network
docker network create --driver bridge isolated_nw

## Build custom docker image
docker-compose build
docker-compose up --build

## Run (Linux)
docker run -it --name teamcity-server-instance -p 8111:8111 -v teamcityserver:/data/teamcity_server/datadir -v teamcityserver:/opt/teamcity/logs
docker run --memory="6g" --cpus=4 -e TEAMCITY_SERVER_MEM_OPTS="-Xmx3g -XX:MaxPermSize=270m -XX:ReservedCodeCacheSize=640m" --name teamcity-server-instance -v C:/TeamCity/data:/data/teamcity_server/datadir -v C:/TeamCity/temp:/opt/teamcity/logs -v C:/TeamCity/temp:/opt/teamcity/temp -p 8111:8111 -e TEAMCITY_HTTPS_PROXY_ENABLED=true jetbrains/teamcity-server
	

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
http://192.168.227.198:8111/

## Get default password (for windows container need to stop it)
docker cp TeamCityServer:C:\TeamCity\logs C:\Temp\TeamCityServer\logs

#Location on host machine (windows)
C:\ProgramData\Docker\volumes

------------------------------------------------- Linux

# Copy docker compose files into the server
mkdir /home/ferdinand/docker-temp
sudo scp ferdinand@10.0.0.129:C:/_Data/GitHub/Docker/docker-samples/src/DockerSamples/TeamCityServer /home/ferdinand/docker-temp/TeamCityServer

# Resources
https://hub.docker.com/r/jetbrains/teamcity-server