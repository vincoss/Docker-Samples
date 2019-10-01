

---------------------------------------------------------------- Basic

## Display Docker version and info
docker --version
docker version
docker info

##Test docker information
docker run hello-world

## List Docker containers (running, all, all in quiet mode)
docker container ls
docker container ls --all
docker container ls -aq

## Docker see running images
docker ps
	

## Stop the container
docker container stop <Container NAME or ID>
	
## Stop all containers
docker container stop $(docker container ls -a -q) 	# powershell
FOR /f "tokens=*" %i IN ('docker ps -q') DO docker stop %i

## Remove all containers
docker container rm $(docker container ls -a -q) 	# powershell
FOR /f "tokens=*" %i IN ('docker ps -q') DO docker rm %i
	
## Docker build the image
docker build --tag=lowercasetagname .






#Swicht containers to linux
& $Env:ProgramFiles\Docker\Docker\DockerCli.exe -SwitchDaemon

#List running containers
docker ps

---------------------------------------------------------------- JIRA config

#Get latest docker container image
docker pull cptactionhank/atlassian-jira-software

#Run|Initialize the container
docker run --detach --publish 8080:8080 cptactionhank/atlassian-jira-software:latest

#Browse to complete he installation
http://localhost:8080/

---------------------------------------------------------------- Example 0001

docker pull microsoft/powershell
docker run --name PSCore -it microsoft/powershell
$PSVersionTable

----------------------------------------------------------------