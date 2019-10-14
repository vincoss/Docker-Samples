# escape=`

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS Basefile

#RUN Install-PackageProvider -Name chocolatey -RequiredVersion 2.8.5.130 -Force; `  
    #Install-Package -Name microsoft-build-tools -RequiredVersion 15.0.26228.0 -Force; `
    #Install-Package -Name netfx-4.5.2-devpack -RequiredVersion 4.5.5165101 -Force; `
    #Install-Package nuget.commandline -RequiredVersion 3.5.0 -Force;

#RUN Install-PackageProvider -Name chocolatey -RequiredVersion 2.8.5.130 -Force; `  
	#Install-Package -Name kdiff -RequiredVersion 0.0.98 -Force; `
	#Install-Package -Name git -RequiredVersion 3.100 -Force; `
	#Install-Package -Name .netMvc5 -RequiredVersion 3.100 -Force; `
	#Install-Package -Name ssrs -RequiredVersion 3.100 -Force; `
	#Install-Package -Name netCore -RequiredVersion 3.100 -Force; `
#    Install-Package -Name 7zip.install -RequiredVersion 19.0.0.0 -Force;
#
# Install 7z CLI
#RUN mkdir "%ProgramFiles%\7-Zip" `
#    && curl -fSLo "%ProgramFiles%\7-Zip\7z.exe" https://www.7-zip.org/a/7z1900-x64.exe

# Set PATH in one layer to keep image size down.
RUN powershell setx /M PATH $(${Env:PATH} `
    + \";${Env:ProgramFiles}\NuGet\" `
    + \";${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\TestAgent\Common7\IDE\CommonExtensions\Microsoft\TestWindow\" `
    + \";${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\" `
    + \";${Env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\" `
    + \";${Env:ProgramFiles(x86)}\Microsoft SDKs\ClickOnce\SignTool\")

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

#docker build --no-cache -t fl/dotnetdev:latest .