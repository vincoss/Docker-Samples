# Resources
https://github.com/portainer/portainer
https://blog.container-solutions.com/

---------------------------------------------------------------- Basic

## Display Docker version and info
docker --version
docker version
docker info

##Test docker information
docker run hello-world

# list images
docker image ls

## List Docker containers (running, all, all in quiet mode)
docker container ls
docker container ls --all
docker container ls -aq

## Docker see running images and all images
docker ps
docker ps -a
	
## Stop the container
docker container stop <Container NAME or ID>
	
## Stop all containers
docker container stop $(docker container ls -a -q) 	# powershell FOR /f "tokens=*" %i IN ('docker ps -q') DO docker stop %i

## Remove all containers
docker container rm $(docker container ls -a -q) 	# powershell FOR /f "tokens=*" %i IN ('docker ps -q') DO docker rm %i
	
## Prune all unused volumes (force)
docker volume prune -f

## Docker build the image
docker build --tag=lowercasetagname .

## View volume data
docker exec jenkins ls /var/jenkins_home

---------------------------------------------------------------- Switch containers Linux|Windows
##Swicht containers to linux
& $Env:ProgramFiles\Docker\Docker\DockerCli.exe -SwitchDaemon

---------------------------------------------------------------- Example 0001

docker pull microsoft/powershell
docker run --name PSCore -it microsoft/powershell
$PSVersionTable

---------------------------------------------------------------- Copy file into volume IN|OUT using dummy container

## Into the volume
docker run -d --rm --name dummy -v myvolume:/root alpine tail -f /dev/null
docker cp c:\myfolder\myfile.txt dummy:/root/myfile.txt
docker stop dummy

# Out of the volume
docker cp dummy:/root/myfile.txt c:\myfolder\myfile.txt

# Copy whole folder out
docker cp dummy:/root/ C:\Temp\TargetDir

---------------------------------------------------------------- Docker Hyper-V

## Fix disk permission problem
https://redmondmag.com/articles/2017/08/02/hyper-v-virtual-hard-disk-permission-problems.aspx