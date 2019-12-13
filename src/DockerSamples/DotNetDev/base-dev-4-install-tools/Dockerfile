# escape=`

FROM jetbrains/teamcity-agent AS base-dev-4-install-tools

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

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

COPY ["./vs_2019_buildtools", "C:/Temp/vs_2019_buildtools/"]

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create images
# docker build --no-cache --force-rm -t vincoss/dotnetdev:base-dev-4-install-tools .

# Run image
#docker container run vincoss/dotnetdev:base-dev-1-install-tools
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-1-install-tools
#docker run --net=isolated_nw --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-1-install-tools
## volume
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 -v tsagent:C:\buildagent\conf vincoss/dotnetdev:base-dev-1-install-tools

