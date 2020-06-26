
## List containers
docker container ls --all

## Stop container
docker container stop webserver

## Remove container
docker container rm webserver

## Container size
docker ps -s

## Run
docker run --name nginx_container -v /var/log nginx