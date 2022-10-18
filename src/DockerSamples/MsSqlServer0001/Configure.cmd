
docker container stop MsSql2019Express
docker container rm MsSql2019Express
docker container prune -f
docker volume prune -f
docker volume create vsql0001

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=z8W#cT0_qp" -e "MSSQL_PID=Express" -e "MSSQL_BACKUP_DIR=/var/opt/mssql/data/backup" -e "MSSQL_DATA_DIR=/var/opt/mssql/data/data" -e "MSSQL_LOG_DIR=/var/opt/mssql/data/logs" -p 11433:1433 --name MsSql2019Express --mount source=V0001,target=/var/opt/mssql/data -d mcr.microsoft.com/mssql/server:2019-CTP3.2-ubuntu

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_PID=Developer" -e "MSSQL_SA_PASSWORD=Pass@word01" -p 1433:1433 --name sqldev2022 -e "MSSQL_BACKUP_DIR=/var/opt/mssql/data/backup" -e "MSSQL_DATA_DIR=/var/opt/mssql/data/data" -e "MSSQL_LOG_DIR=/var/opt/mssql/data/logs" -v c:/sqldev2022:/var/opt/mssql/data -d mcr.microsoft.com/mssql/server:2022-latest

## Resources
https://stackoverflow.com/questions/53623048/restore-database-in-docker-container
https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-configure-environment-variables?view=sql-server-ver16

