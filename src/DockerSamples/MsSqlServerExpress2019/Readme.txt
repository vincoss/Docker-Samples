##Based on
https://hub.docker.com/_/microsoft-mssql-server
https://docs.docker.com/compose/aspnet-mssql-compose/
https://docs.docker.com/compose/reference/up/
https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker?view=sql-server-ver15
https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-environment-variables?view=sql-server-ver15

## Get latest image
docker pull mcr.microsoft.com/mssql/server

## Create volume
docker volume create --name=mssqlserver

## Create network
docker network create --driver bridge isolated_nw

## Build custom docker image
docker-compose build
docker-compose up --build

## Connect and query

sqlcmd -S 192.168.207.204,1433 -U SA -P Pass@word1 -Q "SELECT @@VERSION"

## Tools inside the container
docker exec -it 41748b37a032  "bash"

## Copy all files to client
sudo docker volume inspect mssqlserver
sudo scp -r /var/snap/docker/common/var-lib-docker/volumes/mssqlserver/ ferdinand@10.0.129:c:/temp/ubuntu_core/

------------------------------------------------- Linux

# Copy docker compose files into the server
mkdir /home/ferdinand/mssql-temp
sudo scp -r ferdinand@10.0.0.129:C:/_Data/GitHub/Docker/docker-samples/src/DockerSamples/MsSqlServerExpress2019 /home/ferdinand/mssql-temp/
cd /home/ferdinand/mssql-temp/MsSqlServerExpress2019