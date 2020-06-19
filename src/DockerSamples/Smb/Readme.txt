## Resouces
https://docs.docker.com/compose/compose-file/
https://www.mirantis.com/blog/introduction-to-yaml-creating-a-kubernetes-deployment/
https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/persistent-storage
https://docs.docker.com/storage/volumes/


## Get latest image (widows,linux)
docker pull mcr.microsoft.com/dotnet/core/aspnet:3.1
docker pull mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic

## Create volume
docker volume create --name=docker_tester_website
docker volume create --driver local --opt type=nfs --opt o=nfsvers=4,addr=nfs.example.com,rw --opt device=:G:/SomePath nfs_volume

## Build custom docker image
docker-compose build
docker-compose up --build

## Build & tag
docker build --no-cache -t vincoss/dockerhelper.1.0.0--windows .
docker build -f Dockerfile.ubuntu-x64 --no-cache -t vincoss/dockerhelper.1.0.0-bionic .

## Compose
docker-compose up -d

## Run (windows)
docker run -it --rm -p 8123:80 --name dockerhelper -v G:/:C:/appdata/ dockerhelper.1.0.0-windows
docker run -it --rm -p 8123:80 --name dockerhelper dockerhelper.1.0.0-windows
docker run dockerhelper.1.0.0-windows --name dockerhelper --Urls="http://*:51060;"
docker run dockerhelper.1.0.0-windows --name dockerhelper

## Run (linux)

# Mount folder on the host
docker run -it --rm -p 8123:80 --name dockerhelper -v C:/Temp/Docker/V0/linux-x64/:/appdata vincoss/dockerhelper.1.0.0-bionic

# Mount Shared Drive on the host (does not work on linux for some reason)
docker run -dt -it --rm -p 8123:80 --name dockerhelper -v G:\Azeva\:/appdata:rw vincoss/dockerhelper.1.0.0-bionic

# Volume
docker run -dt -it --rm -p 8123:80 --name dockerhelper -v docker_tester_website:/appdata:rw vincoss/dockerhelper.1.0.0-bionic

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down

## Error logs
docker logs --tail 50 --follow --timestamps dockerhelper

## GetContainer IP
docker inspect -f "{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" dockerhelper

## Browse
http://localhost:8123/api/diagnostics
http://localhost:8123/api/diagnostics/getAppDataFileList
