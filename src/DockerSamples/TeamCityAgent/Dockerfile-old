#FROM mcr.microsoft.com/dotnet/framework/sdk
#FROM devbase:1.0.0-windows
#FROM devbasecore:1.0.0-windows
FROM devbasecoresql:1.0.0-windows
#FROM devbasecoresqlvs:1.0.0-windows

# ------------------------------------------------------------ Base

# Chocolatey
RUN powershell Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco config set --name commandExecutionTimeoutSeconds --value 5400

## 7z
#RUN powershell choco install 7zip.install -y

# ------------------------------------------------------------ Core

# NuGet CLI
#RUN powershell choco install nuget.commandline -y

#  Git
#RUN powershell choco install git -y

## dotnet-framework
#RUN powershell choco install dotnetfx -y

## dotnet
#RUN powershell choco install dotnetcore-sdk -y
#RUN powershell choco install dotnet-5.0-sdk -y

# ------------------------------------------------------------ Sql

#SQL Server Data Tools
#RUN powershell choco install visualstudio2017sql -y

#RUN rm -r "C:\ProgramData\chocolatey\logs"

# ------------------------------------------------------------ VS

## VS Build Tools
#RUN powershell choco install visualstudio2017buildtools --package-parameters "--allWorkloads --includeRecommended --includeOptional --passive" -y
RUN powershell choco install visualstudio2019buildtools --package-parameters "--allWorkloads --includeRecommended --includeOptional --passive" -y
RUN powershell choco install sqlserver-cmdlineutils -y

# ------------------------------------------------------------ VS Build Targes

#RUN mkdir 'C:\temp\Reporting Services'
#RUN mkdir 'C:\temp\Xamarin'
#COPY reporting-services "C:\temp\Reporting Services"
#COPY xamarin "C:\temp\Xamarin"

ARG src="reporting services"
ARG target="C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Reporting Services"
ADD ${src} ${target}

ARG src="xamarin"
ARG target="C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Xamarin"
ADD ${src} ${target}

# ------------------------------------------------------------ User

# Copy files
#COPY createUser.ps1 C:/temp/
#COPY addAccountToLogonAsService.ps1 C:/temp/
#COPY passwordEncrypted.txt C:/temp/

# Create service agent user
#RUN powershell -File .\temp\createUser.ps1

# In order to set system PATH, ContainerAdministrator must be used
#USER ContainerAdministrator
#USER "NT Authority\System"

# ------------------------------------------------------------ TA

#TeamCity Agent
#RUN choco install teamcityagent -y -v -force --allow-empty-checksums --package-parameters="'agentName=teamcityagent0001 serverUrl=http://teamcity:8111/ serviceAccount=.\\TeamCityAgentUser serviceAccountPassword=()_password12'"

##
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
