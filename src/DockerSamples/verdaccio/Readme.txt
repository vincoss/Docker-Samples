

## Create volume
docker volume create --name=verdaccio

## Run
docker run -d -it --name verdaccio-server -p 4873:4873 -v verdaccio:/conf -v verdaccio:/storage -v verdaccio:/plugins verdaccio/verdaccio

http://serverName:4873/

## Resources
https://github.com/verdaccio
https://verdaccio.org/en/
https://www.npmjs.com/