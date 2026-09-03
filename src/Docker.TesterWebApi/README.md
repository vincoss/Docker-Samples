# Docker.TesterWebSite

Helper ASP.NET Core website to test deployed docker container.

### Steps
Publish the API info the folder.

### Build image
```
docker build --force-rm -f Dockerfile.linux -t docker_tester_webapi:test .
docker build --force-rm -f Dockerfile.windows -t docker_tester_webapi_windows:test .
```

### Run
```
podman run --rm -it -p 5202:8080 docker_tester_webapi:test
podman run --rm -it -p 5202:8080 docker_tester_webapi_windows:test
podman run --rm -it -p 5202:8080 -v c:/var/appdata:/var/appdata docker_tester_webapi:test


```

### Browse
```
http://localhost:5202/WeatherForecast
http://localhost:5202/api/diagnostics
```