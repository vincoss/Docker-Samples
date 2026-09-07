
# Readme

``` Build
docker build -f Dockerfile --no-cache -t controllerconsoleapp_docker_sock_windows .
```

### Run
```
docker run -it --rm --name controllerconsoleapp_docker_sock_windows controllerconsoleapp_docker_sock_windows
docker run -it --name controllerconsoleapp_docker_sock_windows -v c:/var/run/docker.sock:/var/run/docker.sock controllerconsoleapp_docker_sock_windows
docker run -it --name controllerconsoleapp_docker_sock_windows -v \\.\pipe\docker_engine:\\.\pipe\docker_engine controllerconsoleapp_docker_sock_windows
docker run -it --name controllerconsoleapp_docker_sock_windows --isolation=process -v \\.\pipe\docker_engine:\\.\pipe\docker_engine controllerconsoleapp_docker_sock_windows

docker run -it --isolation=process -v \\.\pipe\docker_engine:\\.\pipe\docker_engine controllerconsoleapp_docker_sock_windows


```

### Compose
```
docker compose -f docker-compose.yml up -d
```

### Info
Expose daemon on tcp://localhost:2375 without TLS on Setting > General