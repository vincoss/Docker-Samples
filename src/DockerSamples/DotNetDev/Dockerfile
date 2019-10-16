# escape=`

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS Basefile

## Enable https
COPY createUser.ps1 C:/temp/
COPY createPasswordFile.ps1 C:/temp/
COPY passwordEncrypted.txt C:/temp/
RUN powershell -File .\temp\createUser.ps1

# Install packages
RUN powershell Set-ExecutionPolicy Bypass -Scope Process -Force; iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

#RUN choco install 7zip.install -y; `
    #choco install git -y; `
	#choco install dotnetcore-sdk -y; `
	#choco install teamcityagent --params 'serverUrl=http://teamcity:8111/ serviceAccount=TeamCityAgentUser serviceAccountPassword=()_2ac!Kw&3!' -y;
#
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

#RUN Install-PackageProvider -Name chocolatey -RequiredVersion 2.8.5.130 -Force; `
	#Install-Package -Name git -RequiredVersion 2.23.0 -Force; `
	#Install-Package -Name 7zip.install -RequiredVersion 19.0.0.0 -Force;

#RUN Install-PackageProvider -Name chocolatey -RequiredVersion 2.8.5.130 -Force; `  
    #Install-Package -Name microsoft-build-tools -RequiredVersion 15.0.26228.0 -Force; `
    #Install-Package -Name netfx-4.5.2-devpack -RequiredVersion 4.5.5165101 -Force; `
    #Install-Package nuget.commandline -RequiredVersion 3.5.0 -Force;

#RUN choco install dotnetcore-sdk
#RUN Install-PackageProvider -Name chocolatey -RequiredVersion 2.8.5.130 -Force; `  
#	Install-Package -Name 7zip.install -RequiredVersion 19.0.0.0 -Force;
	#Install-Package -Name git -RequiredVersion 3.100 -Force; `
	#Install-Package -Name .netMvc5 -RequiredVersion 3.100 -Force; `
	#Install-Package -Name ssrs -RequiredVersion 3.100 -Force; `
	#Install-Package -Name netCore -RequiredVersion 3.100 -Force; `
#    Install-Package -Name 7zip.install -RequiredVersion 19.0.0.0 -Force;
#
# Install 7z CLI
#RUN mkdir "%ProgramFiles%\7-Zip" `
#    && curl -fSLo "%ProgramFiles%\7-Zip\7z.exe" https://www.7-zip.org/a/7z1900-x64.exe

#RUN powershell get-module

# Set PATH in one layer to keep image size down.
#RUN powershell setx /M PATH $(${Env:PATH} `
    #+ \";${Env:ProgramFiles}\NuGet\" `
    #+ \";${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\TestAgent\Common7\IDE\CommonExtensions\Microsoft\TestWindow\" `
    #+ \";${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\" `
    #+ \";${Env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\" `
    #+ \";${Env:ProgramFiles(x86)}\Microsoft SDKs\ClickOnce\SignTool\")

#RUN powershell setx PATH "$env:path;C:\Program Files\7-Zip" -m
#RUN powershell `
	#$env:Path += ";C:\Program Files\7-Zip" -m

##RUN powershell [Environment]::SetEnvironmentVariable("Path", [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::Machine) + ";C:\temp\glut;C:\temp\gluta", [EnvironmentVariableTarget]::Machine)



# docker build --no-cache -t vincoss/dotnetdev:latest .
# docker container run fl/dotnetdev:latest 
# docker logs --tail 50 --follow --timestamps edb916456deb
# docker run --name TeamCityAgent0001 -h TeamCityAgent0001 --rm -it -p 8000:80 vincoss/dotnetdev:latest


#done
#Install-Package -Name git -RequiredVersion 2.23.0 -Force; `
#Install-Package -Name 7zip.install -RequiredVersion 19.0.0.0 -Force; `