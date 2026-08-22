
# Build and run it

### 1. Build the Docker image
```
docker build -t store-app .
```

### 2. Run it
```
docker run -d --rm -p 5000:8080 --name running-store store-app
```
```
http://localhost:5000/products
```

### Issue 1
```
docker pull mcr.microsoft.com/windows/nanoserver:ltsc2022
ltsc2022: Pulling from windows/nanoserver
no matching manifest for windows(10.0.19045)/amd64 in the manifest list entries
```
Fix
```
Match the specific tag: Use a tag that explicitly matches a supported Nano Server channel (such as ltsc2022 or ltsc2019) rather than a floating or mismatched tag. Check available container tags on the Microsoft Artifact Registry to find a compatible version.
```

https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-10.0