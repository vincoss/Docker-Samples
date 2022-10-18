
## Run Docker Containers without having a logged in User on Windows

1. Open CMD and admnistrator
2. cd C:\Program Files\Docker\Docker\resources
3. dockerd.exe --register-service
4. net start Docker
5. sc config Docker start=auto