
## Run (linux container & on windows)
docker run --rm --name nuget-server -p 5555:80 --env-file baget.env -v "c:/var/baget/:/var/baget" loicsharma/baget:latest

## Resources
https://loic-sharma.github.io/BaGet/installation/docker/