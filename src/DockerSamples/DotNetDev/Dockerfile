FROM mcr.microsoft.com/dotnet/framework/sdk

# ------------------------------------------------------------ Installs Core

# Chocolatey
RUN powershell Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

## 7z
RUN powershell choco install 7zip.install -y

# ------------------------------------------------------------ Installs Dev

# NuGet CLI
RUN powershell choco install nuget.commandline -y

#  Git
RUN powershell choco install git -y

## dotnet
RUN powershell choco install dotnetcore-sdk -y
RUN powershell choco install dotnet-5.0-sdk -y

##
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
