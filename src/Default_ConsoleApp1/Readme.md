
# Readme

``` Build
docker build -f Dockerfile --no-cache -t vincoss/defaultconsoleapp1:1.0.0-windows .
docker build -f Dockerfile.linux --no-cache -t vincoss/defaultconsoleapp1:1.0.0-linux .
```

### Run
```
docker run -it --rm --name defaultconsoleapp1-windows vincoss/defaultconsoleapp1:1.0.0-windows
docker run -it --rm --name defaultconsoleapp1-linux vincoss/defaultconsoleapp1:1.0.0-linux
```

### Compose
```
docker compose -f docker-compose.yml up -d
```

### up (Command)
It downloads (pulls) the required images if they are missing.
It creates the containers, networks, and volumes defined in the file.
It starts the containers in the correct order.

### -d (Flag)
Without -d: Docker locks up your terminal screen. It streams all the logs and console outputs from both containers directly to your window. If you close the terminal or press Ctrl + C, the containers will immediately stop.
With -d: Docker starts the containers, prints their names to confirm they are running, and instantly hands back control of your terminal prompt. Your containers keep running silently in the background.