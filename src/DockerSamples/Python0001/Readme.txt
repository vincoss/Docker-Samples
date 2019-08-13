#Based on
https://docs.docker.com/get-started/part2/

#Steps
cd to target directory
dir to list files

# build docker image
docker build --tag=friendlyhello_v .
OR
docker build --tag=friendlyhello_v:v0.0.1 .

# list images
docker image ls

#run container
docker run -p 4000:80 --name friendlyhello_v -d friendlyhello_v:latest
OR
docker run -p 4000:80 --name friendlyhello_v -d friendlyhello_v:v0.0.1

#Open browser
http://localhost:4000/
