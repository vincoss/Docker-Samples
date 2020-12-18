##Based on
https://hub.docker.com/r/jetbrains/teamcity-agent
https://hub.docker.com/_/microsoft-dotnet-framework
https://hub.docker.com/_/microsoft-dotnet-core-sdk/
https://hub.docker.com/_/microsoft-dotnet-sdk

https://visualstudio.microsoft.com/downloads/
https://docs.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio?view=vs-2019
https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility?view=sql-server-ver15
https://docs.microsoft.com/en-us/visualstudio/install/build-tools-container?view=vs-2019#step-5-create-and-build-the-dockerfile
https://docs.microsoft.com/en-us/visualstudio/install/workload-component-id-vs-build-tools?view=vs-2019
https://docs.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio?view=vs-2019

## Get latest image
docker pull jetbrains/teamcity-agent

## Create volume
docker volume create --name=teamcityagent

## Create network
docker network create --driver nat isolated_nw

## Build & tag
docker build -f Dockerfile --no-cache -m 2GB -t devbase:1.0.0-windows .
docker build -f Dockerfile --no-cache -m 2GB -t devbasecore:1.0.0-windows .
docker build -f Dockerfile --no-cache -m 2GB -t devbasecoresql:1.0.0-windows .
docker build -f Dockerfile --no-cache -m 16GB -t devbasecoresqlvs:1.0.0-windows .
docker build -f Dockerfile --no-cache -m 2GB -t devbasecoresqlvsta:1.0.0-windows .

## Run
docker run -it --rm -h teamcityagent0001 --name teamcityagent0001 -v c:/buildagent/conf:c:/var/teamcity/teamcityagent0001/conf -v c:/buildagent/logs:c:/var/teamcity/teamcityagent0001/logs devbase:1.0.0-windows
docker run -it --rm --name teamcityagent0001 devbasecoresqlvsta:1.0.0-windows
docker run -it --rm -h teamcityagent0001 --name teamcityagent0001 devbasecoresqlvs:1.0.0-windows


## Build custom docker image
docker-compose build
docker-compose up -d
docker-compose up --build -d
docker-compose -f docker-compose.yml up -d
docker-compose up --build -t teamcityagentdev:1.0.0-windows .

##docker build -f src/Gnu.Licensing.DevResources/Docker/Dockerfile.win-x64 --no-cache -t vincoss/gnulicensesvr:1.0.0-windows .

## Compose (scale agents)
docker-compose up -d
docker-compose up -d scale 2 # todo review

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down -v

## Error logs
docker inspect teamcityagent
docker logs --tail 50 --follow --timestamps teamcityagent

## Show running container IP
docker inspect -f "{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" teamcityagent
docker exec -it teamcityagent0001 cmd

## Browse
http://localhost:8111/

## Get default password
docker cp jenkins:/var/jenkins_home/secrets/initialAdminPassword C:\Temp\Jenkins

------------------------------------------------- Create agent

1. create volume
docker volume create --name=teamcityagent

2. create network (if not exists)
docker network create --driver ics isolated_nw
docker network create -driver macvlan --subnet=100.98.26.43/24 --gateway=100.98.26.1  -o parent=eth0 pub_net

2. compose container
docker-compose up -d




    #build:
    #  context: .
    #  dockerfile: Dockerfile

## Resources
https://github.com/actions/virtual-environments