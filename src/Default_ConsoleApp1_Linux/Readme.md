
### Docker build image
```
podman build --force-rm -f Dockerfile -t defaultconsoleapp1linux:test .
docker build --force-rm -f Dockerfile -t defaultconsoleapp1linux:test .
```

### Docker run
```
podman run --rm -it defaultconsoleapp1linux:test
podman run --rm -d defaultconsoleapp1linux:test
```

## Compose
```
docker compose create
docker compose build
docker compose up

docker compose -f docker-compose.yml up -d
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