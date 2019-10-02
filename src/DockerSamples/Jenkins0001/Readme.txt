##Based on
https://hub.docker.com/r/jenkins/jenkins
https://github.com/jenkinsci/docker/blob/master/README.md

#yml|yaml
https://docs.docker.com/compose/compose-file/
https://www.mirantis.com/blog/introduction-to-yaml-creating-a-kubernetes-deployment/


## Compose
docker-compose up -d

## Show containers
docker ps -a

## Stop and remove containers
docker-compose down

## Create volume
docker volume create --name=data

## Error logs
docker logs --tail 50 --follow --timestamps jenkins

## Browse
http://localhost:8111/