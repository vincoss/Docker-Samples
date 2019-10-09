#	7 zip
#	.net 4.7 sdk
#	.net 4.8 sdk
#	.asp.net mvc 5 skd
#.sql cmd tools
#	.net core sdk
#	.VS build tools 2019
# SSRS tools
# python
#add bin file environment paths
#limit teamcity memory and cpu
#limit agent memory and cpu
#sql 
#notepad++
#powershell
#configure admin center


FROM teamcity-agent:latest-windowsservercore-1809 AS buildagent

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]


# SKDs
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8
FROM mcr.microsoft.com/dotnet/core/sdk:2.1
FROM mcr.microsoft.com/dotnet/core/sdk:3.0
##FROM mcr.microsoft.com/mssql-tools	thats linux
##FROM python

# Install 7z
#RUN [Net.ServicePointManager]::SecurityProtocol = 'tls12, tls11, tls' ; \
    #Invoke-WebRequest https://www.7-zip.org/a/7z1900-x64.msi -OutFile 7z.msi; \
    #Start-Process msiexec -Wait -ArgumentList /q, /i, 7z.msi ; \
    #Remove-Item -Force 7z.msi

# Install 7z
#RUN [Net.ServicePointManager]::SecurityProtocol = 'tls12, tls11, tls' ; \
    #Invoke-WebRequest https://www.7-zip.org/a/7z1900-extra.7z -OutFile 7z.zip; \
    #Expand-Archive 7z.zip -DestinationPath $Env:ProgramFiles\7-Zip ; \
    #Remove-Item -Force 7z.zip


# Path
RUN setx /M Path "C:\Program Files\7-Zip"