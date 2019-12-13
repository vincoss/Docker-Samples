##Based on
https://hub.docker.com/_/microsoft-dotnet-framework
https://hub.docker.com/_/microsoft-dotnet-core-sdk/
https://github.com/microsoft/dotnet-framework-docker


# Build docker image
docker build --no-cache -t vincoss/dotnetdev:1.0.0 .

## Issues

1. Installing TeamCity Agent might fail if the service user does not have righs to "Log on as service".
	https://superuser.com/questions/747094/easiest-way-to-grant-log-on-as-a-service-to-a-windows-user-from-the-command-li

	
## Steps to create the image

1. cd to this directory
2. Create password bu running 'createPasswordFile.ps1' script file. This is on local machine. 