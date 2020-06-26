##Based on
https://hub.docker.com/_/microsoft-mssql-server
https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker?view=sql-server-ver15

## Get latest image
docker pull mcr.microsoft.com/mssql/server

## Create volume
docker volume create --name=mssqlserver

## Create network
docker network create --driver bridge isolated_nw

## Build custom docker image
docker-compose build
docker-compose up --build

------------------------------------------------- Linux

# Copy docker compose files into the server
mkdir /home/ferdinandgad/mssql-temp
sudo scp -r ferdinand@10.0.0.129:C:/_Data/GitHub/Docker/docker-samples/src/DockerSamples/MsSqlServerExpress2019 /home/ferdinandgad/mssql-temp/