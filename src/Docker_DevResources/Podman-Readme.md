# Podman

### Error - podman running container api not working ERR_CONNECTION_REFUSED
```
Run this command
podman machine ssh ip route show | Select-String "src"

use the IP instead of localhost

-Fix
wsl --shutdown
wsl --list --verbose
wsl --set-default podman-machine-default
podman machine rm podman-machine-default --force
podman machine init
podman machine start

```

 ### Resources
 https://docs.podman.io/en/v5.5.2/markdown/podman-build.1.html