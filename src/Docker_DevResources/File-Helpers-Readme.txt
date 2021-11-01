
## User & Machine name
@echo off
WHOAMI
cmd /k hostname

## robocopy samples
robocopy /s /mir "c:\temp\new-folder" "d:\temp\new-folder"
IF %%ERRORLEVEL%% LEQ 4 exit /B 0

#exclude Archive folder
robocopy /s /mir "c:\temp\new-folder" "d:\temp\new-folder" /XD Archive
IF %%ERRORLEVEL%% LEQ 4 exit /B 0

## xcopy samples

xcopy /y /S "c:\temp\new-folder\**" "d:\temp\new-folder\"
xcopy /y /S "c:\temp\new-folder\File.xml" "d:\temp\new-folder\"

## del samples

del /q "c:\temp\new-folder\*"

## rmdir samples

rmdir /Q /S "add"
rmdir /Q /S "c:\temp\new-folder"

## MKDIR samples
MKDIR "c:\temp\new-folder"

## 7z samples
7z e -y File-Name.7z -o"C:\Temp"

7z a "c:\FileName.7z" "C:\Temp\*"
7z a "c:\FileName.7z" "C:\Temp\*" -r -x!ConnectionStrings.config -x!ProjectName.snk -x!packages.config -xr!App_Data\Log -x!Log4Net.config

## CI NPM build tools
npm install --global --production windows-build-tools
npm install --global node-gyp
npm config set python "%USERPROFILE%\.windows-build-tools\python27\python.exe"

## The RPC server is unavailable.
netsh advfirewall firewall add rule name="COM+ (DCOM-In) Port Mapper" dir=in action=allow description="Allow Communication to the DCOM Service Control Manager" enable=yes localport=135 protocol=tcp
netsh advfirewall firewall add rule name="COM+ (DCOM-In) Dynamic Port Range" dir=in action=allow description="Allow DCOM Communication" enable=yes localport=RPC protocol=tcp

netsh advfirewall firewall add rule name="COM+ (DCOM-In) Port Mapper" dir=in action=allow description="Allow Communication to the DCOM Service Control Manager" enable=yes localport=135 protocol=tcp remoteip=x.x.x.x
netsh advfirewall firewall add rule name="COM+ (DCOM-In) Dynamic Port Range" dir=in action=allow description="Allow DCOM Communication" enable=yes localport=RPC protocol=tcp remoteip=x.x.x.x

## iisreset IIS
iisreset Server-Name /stop
iisreset Server-Name /start
sc Server-Name stop iisadmin

## SqlPackage.exe
https://docs.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download?view=sql-server-ver15
Install location: C:\Program Files\Microsoft SQL Server\150\DAC\bin
SqlPackage.exe /Action:Publish /SourceFile:"ProjectName.dacpac" /Profile:"DB-Project-Name.deploy.xml"

## Hyper-V
Install-WindowsFeature -Name Hyper-V -ComputerName Todo-Server-Name -IncludeManagementTools -Restart
Get-WindowsFeature -ComputerName Todo-Server-Name

## Windows Service (PowerShell)
New-Service -Name "Service-Name" "C:\Program Files (x86)\Folder\WindowsService.exe" -Description "Service-Name" -DisplayName "Service-Name" -StartupType Automatic
sc.exe delete "Windows-Service-Name"

## EntityFramework
dotnet **/Release/net5.0/publish/ProjectName.dll --environment=development --ef-migrate
dotnet ef database update --project ProjectName
