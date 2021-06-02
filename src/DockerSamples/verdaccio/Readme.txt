

## Create volume
docker volume create --name=verdaccio

## Run
docker run -d -it --name verdaccio-server -p 4873:4873 -v verdaccio:/conf -v verdaccio:/storage -v verdaccio:/plugins verdaccio/verdaccio

http://serverName:4873/

## Resources
https://github.com/verdaccio
https://verdaccio.org/en/
https://www.npmjs.com/

## Create package, publish use package

## Configure registry for NPM publishing
npm install --registry http://bnewks009:4873/
npm set init.registry http://bnewks009:4873/

## Adding a Verdaccio user
npm adduser --registry http://bnewks009:4873

# Publich package
npm publish --registry http://bnewks009:4873

# Remove publish package
npm unpublish --force packagename

# Resoucess
https://www.youtube.com/watch?v=Co0RwdpEsag
https://github.com/verdaccio/verdaccio/issues/212