#Based on
https://docs.docker.com/get-started/part2/
https://docs.docker.com/get-started/part3/

------------------------------------------------------------------------------------------------------ Part 2: Containers

#Steps
cd to target directory
dir to list files

# Create image using this directory's Dockerfile
docker build --tag=friendlyhello_v .
OR
docker build --tag=friendlyhello_v:v0.0.1 .

# list images
docker image ls

# list containers
docker container ls
docker container ls -a	# Include not running containers

# Run "friendlyhello" mapping port 4000 to 80
docker run -p 4000:80 --name friendlyhello_v friendlyhello_v:latest
docker run -p 4000:80 --name friendlyhello_v -d friendlyhello_v:latest	# in detached mode
OR
docker run -p 4000:80 --name friendlyhello_v -d friendlyhello_v:v0.0.1

# stop and remove container
docker container stop friendlyhello_v
docker container kill friendlyhello_v
docker container rm friendlyhello_v

# Tag image before publish to Docker public registry
docker tag friendlyhello_v ferdinand/get-started:part2

# Publish the image
docker push ferdinand/get-started:part2

# Pull and run docker image
docker run -p 4000:80 ferdinand/get-started:part2

# Hint
CTRL+C in terminal to quit

# Cheat Sheet
docker build -t friendlyhello .						# Create image using this directory's Dockerfile
docker run -p 4000:80 friendlyhello					# Run "friendlyhello" mapping port 4000 to 80
docker run -d -p 4000:80 friendlyhello				# Same thing, but in detached mode
docker container ls									# List all running containers
docker container ls -a								# List all containers, even those not running
docker container stop <hash>						# Gracefully stop the specified container
docker container kill <hash>						# Force shutdown of the specified container
docker container rm <hash>							# Remove specified container from this machine
docker container rm $(docker container ls -a -q)    # Remove all containers
docker image ls -a									# List all images on this machine
docker image rm <image id>							# Remove specified image from this machine
docker image rm $(docker image ls -a -q)			# Remove all images from this machine
docker login										# Log in this CLI session using your Docker credentials
docker tag <image> username/repository:tag			# Tag <image> for upload to registry
docker push username/repository:tag					# Upload tagged image to registry
docker run username/repository:tag                  # Run image from a registry

# Open browser
http://localhost:4000/
http://172.17.0.2:4000/

------------------------------------------------------------------------------------------------------ Part 3: Services

## Init docker swarm
docker swarm init

## Run docker images with specified name
docker stack deploy -c docker-compose.yml getstartedlab

## Get service name
docker service ls
docker stack services getstartedlab # Alternatively, you can run
docker service ps getstartedlab_web
docker stack ps getstartedlab

## Take down the app and swarm

docker stack rm getstartedlab
docker swarm leave --force

## Cheat Sheet

docker stack ls                                 # List stacks or apps
docker stack deploy -c <composefile> <appname>  # Run the specified Compose file
docker service ls								# List running services associated with an app
docker service ps <service>						# List tasks associated with an app
docker inspect <task or container>              # Inspect task or container
docker container ls -q                          # List container IDs
docker stack rm <appname>                       # Tear down an application
docker swarm leave --force						# Take down a single node swarm from the manager