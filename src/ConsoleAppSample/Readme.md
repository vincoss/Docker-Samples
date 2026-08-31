
# Readme

NOTE: The samples are written for cmd.exe. Publish the application first.

### Build & tag
```
docker build -f Dockerfile.windows --no-cache -t vincoss/consoleappsample:1.0.0-windows .
docker build -f Dockerfile.ubuntu --no-cache -t vincoss/consoleappsample:1.0.0-bionic .
```

### Run
```
docker run -it --rm --name consoleappsample -v c:/var/appdata:c:/var/appdata vincoss/consoleappsample:1.0.0-windows
docker run -it --rm --name consoleappsample -v c:/var/appdata:/var/appdata vincoss/consoleappsample:1.0.0-bionic
```

### Run wth arguments
To pass command-line arguments to a container using docker run -it, append the arguments at the very end of the command, right after the image name.

```
docker run -it <image_name> [arguments_here]
docker run -it --rm --name consoleappsample -v c:/var/appdata:c:/var/appdata vincoss/consoleappsample:1.0.0-windows --environment Development
docker run -it --rm --name consoleappsample -v c:/var/appdata:/var/appdata vincoss/consoleappsample:1.0.0-bionic --environment Development
```

### Paths
```
/opt: Used for standalone, self-contained, or proprietary software packages that keep their files in one place.
```

### Resources
https://refspecs.linuxfoundation.org/FHS_3.0/fhs/ch05.html
https://www.docker.com/blog/docker-best-practices-choosing-between-run-cmd-and-entrypoint
https://github.com/microsoft/mssql-docker/tree/master/linux
https://github.com/nreco/logging