##Based on
https://hub.docker.com/r/jenkins/jenkins
https://github.com/jenkinsci/docker/blob/master/README.md
https://code-maze.com/ci-jenkins-docker/

#yml|yaml
https://docs.docker.com/compose/compose-file/
https://www.mirantis.com/blog/introduction-to-yaml-creating-a-kubernetes-deployment/


## Get latest image
docker pull jenkins/jenkins:lts

## Create volume
docker volume create --name=jenkins_home

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