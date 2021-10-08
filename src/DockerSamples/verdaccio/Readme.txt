
## Create volume
docker volume create --name=verdaccio

## Run
docker run -d --restart always -it --name verdaccio-server -p 4873:4873 -v verdaccio:/conf -v verdaccio:/storage -v verdaccio:/plugins verdaccio/verdaccio

http://serverName:4873/

## Configure local registry for NPM publishing
npm install --registry http://bnewks009:4873/
npm set init.registry http://bnewks009:4873/
verdaccio

# Uninstall (local)
npm uninstall -g verdaccio

## Adding a Verdaccio user
npm adduser --registry http://bnewks009:4873

# Publich package
npm publish --registry http://bnewks009:4873

# Remove publish package (project)
npm unpublish --force packagename --registry http://bnewks009:4873

# Remove publish package specific version
npm unpublish myModule@1.2.3 --registry http://bnewks009:4873

# Resoucess
https://verdaccio.org/
https://github.com/verdaccio/verdaccio
https://hub.docker.com/r/verdaccio/verdaccio
https://github.com/verdaccio/verdaccio/issues/212
https://verdaccio.org/docs/en/configuration.html
https://www.youtube.com/watch?v=Co0RwdpEsag
https://www.npmjs.com/

## Issues

# Fix fix max upload size need modify config and apply changes

# download
docker cp cf6d83490524:/verdaccio/conf/config.yaml C:\Temp\verdaccio
# upload
docker cp C:\Temp\verdaccio\config.yaml cf6d83490524:/verdaccio/conf/config.yaml