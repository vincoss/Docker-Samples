## Resources
https://docs.docker.com/storage/volumes/

## Create volume
docker volume create test

## List volumesw
docker volume ls

## Inspect volume
docker volume inspect test

## Remove volume
docker volume rm test

## Start container with volume
docker run -d --name devtest --mount source=myvol2,target=/app nginx:latest

## Start a service with volumes
docker service create -d --replicas=4 --name devtest-service --mount source=myvol2,target=/app nginx:latest

# Read only
docker run -d --name=nginxtest --mount source=nginx-vol,destination=/usr/share/nginx/html,readonly nginx:latest

# check service is running
docker service ps devtest-service

# remove service
docker service rm devtest-service

## Use a volume driver
docker volume create --driver vieux/sshfs -o sshcmd=ferdinand@10.0.129:c:/temp/docker -o password=pwdhere sshvolume

# Run container with volume
docker run -dt -it --rm -p 8123:80 --name dockerhelper -v sshvolume:/appdata:rw vincoss/dockerhelper.1.0.0-bionic