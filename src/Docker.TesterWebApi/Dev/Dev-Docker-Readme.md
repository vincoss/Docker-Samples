# Readme

NOTE:
The samples are written for cmd.exe.

## Build & tag
docker build -f Dockerfile.windows-x64 --no-cache -t vincoss/dockertesterapi:1.0.0-windows .
docker build -f Dockerfile.ubuntu-x64 --no-cache -t vincoss/dockertesterapi:1.0.0-bionic .
docker build -f Dockerfile.ubuntu-arm64 --no-cache -t vincoss/dockertesterapi:1.0.0-bionic-arm64 .

## Push to docker hub (Optional)
docker image push vincoss/dockertesterapi:1.0.0-windows
docker image push vincoss/dockertesterapi:1.0.0-bionic
docker image push vincoss/dockertesterapi:1.0.0-bionic-arm64

## Run
docker run -it --rm -p 8003:8080 --name dockertesterapi -h dockertester -v dockertester:c:/_dockerdata/docker.testerwebapi vincoss/dockertesterapi:1.0.0-windows

## Run Windows using Linux contaners
docker run -it --rm -p 8003:8080 --name dockertesterapi -h dockertester -v c:/_dockerdata/docker.testerwebapi/:/var/appdata vincoss/dockertesterapi:1.0.0-bionic
docker run -it --rm -p 8003:8080 --name dockertesterapi -h dockertester -v c:/_dockerdata/docker.testerwebapi/:/var/appdata vincoss/dockertesterapi:1.0.0-bionic-arm64

## Error logs
docker logs --tail 50 --follow --timestamps dockertesterapi

## Show running container IP
docker inspect -f "{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" dockertesterapi


### Windows Image Tags and Versions

```
9.0-nanoserver-1809:		Built for Windows Server 2018/1809 base layers.
9.0-nanoserver-ltsc2022:	Built for Windows Server 2022 containers.
9.0-nanoserver-ltsc2025:	Built for Windows Server 2025 host
```


##------------------------------------------------ Test

## Browse
http://localhost/api/diagnostics
http://dockertester/api/diagnostics
http://localhost:8003/api/diagnostics
http://localhost:8003/api/diagnostics/getAppDataFileList
http://localhost:8003/api/diagnostics/getNetworkInfo
http://localhost:8003/api/certificate
http://localhost:8003/api/certificate/FindCertificate

http://{ip-here}/api/diagnostics

### Resources
https://hub.docker.com/_/microsoft-dotnet-core-aspnet/
https://hub.docker.com/r/microsoft/dotnet
https://github.com/dotnet/dotnet-docker/blob/main/README.runtime.md