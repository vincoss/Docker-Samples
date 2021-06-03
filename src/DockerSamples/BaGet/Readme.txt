
## Run (linux container & on windows)
docker run --rm --name nuget-server -p 5555:80 --env-file baget.env -v "c:/var/baget/:/var/baget" loicsharma/baget:latest

# Remove package
nuget delete MyPackage 1.0 -Source http://10.0.0.29:5003/ -ApiKey API-KEY

## Resources
https://loic-sharma.github.io/BaGet/
https://loic-sharma.github.io/BaGet/installation/docker/