---------------------------------------------------------------- JIRA config

#Get latest docker container image
docker pull cptactionhank/atlassian-jira-software

#Run|Initialize the container
docker run --detach --publish 8080:8080 cptactionhank/atlassian-jira-software:latest

#Browse to complete he installation
http://localhost:8080/
