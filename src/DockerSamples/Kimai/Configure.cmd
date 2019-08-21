
docker container stop kimai2
docker container rm kimai2
docker container prune -f --filter="label=kimai2"
docker volume prune -f --filter="label=kimai2"
docker volume create Volume_kimai2

docker run -p 8001:8001 --name kimai2 --mount source=Volume_kimai2,target=/mysql:/var/lib/mysql -d kimai/kimai2:latest
 