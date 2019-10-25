# escape=`

FROM mcr.microsoft.com/windows/servercore:ltsc2019 AS base-dev-1-install-tools

# Copy files
COPY createUser.ps1 C:/temp/
COPY addAccountToLogonAsService.ps1 C:/temp/
COPY passwordEncrypted.txt C:/temp/

# Create service agent user
RUN powershell -File .\temp\createUser.ps1

# Install packages
RUN powershell Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco install -y 7zip.install
RUN choco install -y git
RUN choco install teamcityagent -y -v -force --allow-empty-checksums --package-parameters="'serverUrl=http://teamcity:8111/ serviceAccount=.\\TeamCityAgentUser serviceAccountPassword=()_password12'"
#
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create images
# docker build --no-cache --force-rm -t vincoss/dotnetdev:base-dev-1-install-tools .

# Run image
#docker container run vincoss/dotnetdev:base-dev-1-install-tools
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-1-install-tools
#docker run --net=isolated_nw --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-1-install-tools