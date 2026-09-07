
# Readme

``` Build
docker build -f Dockerfile --no-cache -t workerconsolesample_windows .
```

### Run
```
docker run -it --rm --name workerconsolesample_windows-windows workerconsolesample_windows
```

### Compose
```
docker compose -f docker-compose.yml up -d
```