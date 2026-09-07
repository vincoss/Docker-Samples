
### Build image
```
podman build --force-rm -f Dockerfile -t defaultwebapplication1linux:test .
docker build --force-rm -f Dockerfile -t defaultwebapplication1linux:test .
```

### Run
```
podman run --rm -it -p 5202:8080 defaultwebapplication1linux:test
podman run --rm -d  -p 5202:8080 defaultwebapplication1linux:test
```

### Description
```
--force-rm      Remove Intermediate Containers During Build
-t              Specifies the name which is assigned to the resulting image if the build process completes successfully. 
-f              Specifies a Containerfile which contains instructions for building the image.
--rm            Run and automatically remove the container when it exits.
-it             Run interactively with a terminal (great for debugging or exploration).
-d              Run in the background (detached)
-p				Command to map a port on your host machine to a port inside the container. Order matters: It is always Host:Container.

**Example**
Restrict Traffic to Localhost (Secure)
By default, -p opens the port to your entire network (0.0.0.0). 
To make it accessible only from your actual host computer, bind it to 127.0.0.1

docker run -p 127.0.0.1:8080:80 nginx

```



### Resources
```
https://docs.podman.io/en/v5.5.2/markdown/podman-build.1.html
https://hub.docker.com/r/microsoft/dotnet-aspnet
```

### Browse
```
http://localhost:5202/WeatherForecast
```