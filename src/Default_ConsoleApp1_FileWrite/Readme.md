
### Docker build image
```
docker build --force-rm -f Dockerfile.windows -t default_consoleapp1_filewrite:test .
```

### Docker run
```
docker run --rm -it default_consoleapp1_filewrite:test
docker run --rm -d default_consoleapp1_filewrite:test
```

## Compose
```
docker compose create
docker compose build
docker compose up
```

### Description
```
--force-rm      Remove Intermediate Containers During Build
-t              Specifies the name which is assigned to the resulting image if the build process completes successfully. 
-f              Specifies a Containerfile which contains instructions for building the image.
--rm            Run and automatically remove the container when it exits.
-it             Run interactively with a terminal (great for debugging or exploration).
-d              Run in the background (detached)
```

### Resources
```
https://docs.podman.io/en/v5.5.2/markdown/podman-build.1.html
```