# escape=`

#hcsshim::PrepareLayer - failed failed in Win32: The parameter is incorrect. (0x57)
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2019 AS base-dev-1-install-tools
#FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-1803 AS base-dev-1-install-tools

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

# Install TeamCity agent
RUN choco install teamcityagent -y -v -force --allow-empty-checksums --package-parameters="'serverUrl=http://teamcity:8111/ serviceAccount=.\\TeamCityAgentUser serviceAccountPassword=()_password12'"

# Create volume
VOLUME C:/BuildAgent/conf

#
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create images
# docker build --no-cache --force-rm -t vincoss/dotnetdev:base-dev-1-install-tools .

# Run image
#docker container run vincoss/dotnetdev:base-dev-1-install-tools
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-1-install-tools


## volume & size
#docker run --storage-opt "size=100GB" --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 -v teamcity-agent:C:/BuildAgent/conf vincoss/dotnetdev:base-dev-1-install-tools


