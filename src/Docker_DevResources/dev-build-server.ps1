# Install ASP.NET
Install-WindowsFeature NET-Framework-45-ASPNET
Install-WindowsFeature Web-Asp-Net45

New-Item -ItemType directory -Path "C:\tools"
 
# Download and install MSBuild 12
Invoke-WebRequest -Uri "https://download.microsoft.com/download/9/B/B/9BB1309E-1A8F-4A47-A6C5-ECF76672A3B3/BuildTools_Full.exe" -OutFile "C:\tools\BuildTools_Full_12.exe" -UseBasicParsing
["C:\\tools\\BuildTools_Full_12.exe", "/Silent", "/Full"]

# Download and install MSBuild 14
Invoke-WebRequest "https://download.microsoft.com/download/E/E/D/EEDF18A8-4AED-4CE0-BEBE-70A83094FC5A/BuildTools_Full.exe" -OutFile "C:\tools\BuildTools_Full_14.exe" -UseBasicParsing
["C:\\tools\\BuildTools_Full_14.exe", "/Silent", "/Full"]

# Download and install MSBuild 17
Invoke-WebRequest "https://download.microsoft.com/download/A/6/3/A637DB94-8BA8-43BB-BA59-A7CF3420CD90/vs_BuildTools.exe" -OutFile "C:\tools\vs_BuildTools.exe" -UseBasicParsing
["C:\\tools\\vs_buildtools.exe", "--add Microsoft.VisualStudio.Workload.MSBuildTools", "--passive", "--norestart"]

# Note: Install .Net 4.5.2 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/4/3/B/43B61315-B2CE-4F5B-9E32-34CCA07B2F0E/NDP452-KB2901951-x86-x64-DevPack.exe" -OutFile "C:\tools\NDP452-KB2901951-x86-x64-DevPack.exe" -UseBasicParsing
["C:\\tools\\NDP452-KB2901951-x86-x64-DevPack.exe", "/q", "/norestart"]

# Note: Install .Net 4.6.2 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/E/F/D/EFD52638-B804-4865-BB57-47F4B9C80269/NDP462-DevPack-KB3151934-ENU.exe" -OutFile "C:\tools\NDP462-DevPack-KB3151934-ENU.exe" -UseBasicParsing
["C:\\tools\\NDP462-DevPack-KB3151934-ENU.exe", "/q", "/norestart"]

# Note: Install .Net 4.7 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/A/1/D/A1D07600-6915-4CB8-A931-9A980EF47BB7/NDP47-DevPack-KB3186612-ENU.exe" -OutFile "C:\tools\NDP47-DevPack-KB3186612-ENU.exe" -UseBasicParsing
["C:\\tools\\NDP47-DevPack-KB3186612-ENU.exe", "/q", "/norestart"]
 
Invoke-WebRequest -Uri "https://download.microsoft.com/download/F/9/1/F91B5312-4385-4476-9688-055E3B1ED10F/windowssdk/winsdksetup.exe" -OutFile "C:\tools\winsdksetup.exe" -UseBasicParsing
["C:\\tools\\winsdksetup.exe", "/q", "/norestart"]

# Download DotNet Core
Invoke-WebRequest "https://download.microsoft.com/download/E/7/8/E782433E-7737-4E6C-BFBF-290A0A81C3D7/dotnet-dev-win-x64.1.0.4.zip" -OutFile "C:\tools\dotnet-dev-win-x64.1.0.4.zip" -UseBasicParsing
Expand-Archive "C:\tools\dotnet-dev-win-x64.1.0.4.zip" -DestinationPath "C:\tools\dotnet"

# Download NuGet
Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "C:\tools\nuget.exe" -UseBasicParsing

# Download NUnit
Invoke-WebRequest "https://github.com/nunit/nunit-console/releases/download/3.6.1/NUnit.Console-3.6.1.zip" -OutFile "C:\tools\NUnit.Console-3.6.1.zip" -UseBasicParsing
Expand-Archive "C:\tools\NUnit.Console-3.6.1.zip" -DestinationPath "C:\tools\nunit"

# Install Web Targets
WORKDIR "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0"

["C:\\tools\\nuget.exe", "Install MSBuild.Microsoft.VisualStudio.Web.targets -Version 12.0.4"]
mv 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0\MSBuild.Microsoft.VisualStudio.Web.targets.12.0.4\tools\VSToolsPath\*' 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0\'

WORKDIR "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0"

["C:\\tools\\nuget.exe", "Install MSBuild.Microsoft.VisualStudio.Web.targets -Version 14.0.0.3"]
mv 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3\tools\VSToolsPath\*' 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\'

# Set paths
setx PATH '%PATH%;C:\tools;C:\tools\dotnet;C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\;C:\tools\nuget.exe'
setx MSBuildSDKsPath 'C:\tools\dotnet\sdk\1.0.4\Sdks'

# Install ASP.NET
Install-WindowsFeature NET-Framework-45-ASPNET
Install-WindowsFeature Web-Asp-Net45
 
New-Item -ItemType directory -Path "C:\tools"
 
# Download and install MSBuild 12
Invoke-WebRequest -Uri "https://download.microsoft.com/download/9/B/B/9BB1309E-1A8F-4A47-A6C5-ECF76672A3B3/BuildTools_Full.exe" -OutFile "C:\tools\BuildTools_Full_12.exe" -UseBasicParsing
["C:\\tools\\BuildTools_Full_12.exe", "/Silent", "/Full"]
 
# Download and install MSBuild 14
Invoke-WebRequest "https://download.microsoft.com/download/E/E/D/EEDF18A8-4AED-4CE0-BEBE-70A83094FC5A/BuildTools_Full.exe" -OutFile "C:\tools\BuildTools_Full_14.exe" -UseBasicParsing
["C:\\tools\\BuildTools_Full_14.exe", "/Silent", "/Full"]
 
# Download and install MSBuild 17
Invoke-WebRequest "https://download.microsoft.com/download/A/6/3/A637DB94-8BA8-43BB-BA59-A7CF3420CD90/vs_BuildTools.exe" -OutFile "C:\tools\vs_BuildTools.exe" -UseBasicParsing
["C:\\tools\\vs_buildtools.exe", "--add Microsoft.VisualStudio.Workload.MSBuildTools", "--passive", "--norestart"]
 
# Note: Install .Net 4.5.2 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/4/3/B/43B61315-B2CE-4F5B-9E32-34CCA07B2F0E/NDP452-KB2901951-x86-x64-DevPack.exe" -OutFile "C:\tools\NDP452-KB2901951-x86-x64-DevPack.exe" -UseBasicParsing
["C:\\tools\\NDP452-KB2901951-x86-x64-DevPack.exe", "/q", "/norestart"]
 
# Note: Install .Net 4.6.2 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/E/F/D/EFD52638-B804-4865-BB57-47F4B9C80269/NDP462-DevPack-KB3151934-ENU.exe" -OutFile "C:\tools\NDP462-DevPack-KB3151934-ENU.exe" -UseBasicParsing
["C:\\tools\\NDP462-DevPack-KB3151934-ENU.exe", "/q", "/norestart"]
 
# Note: Install .Net 4.7 Developer/Targeting Pack
Invoke-WebRequest -Uri "https://download.microsoft.com/download/A/1/D/A1D07600-6915-4CB8-A931-9A980EF47BB7/NDP47-DevPack-KB3186612-ENU.exe" -OutFile "C:\tools\NDP47-DevPack-KB3186612-ENU.exe" -UseBasicParsing
["C:\\tools\\NDP47-DevPack-KB3186612-ENU.exe", "/q", "/norestart"]
 
Invoke-WebRequest -Uri "https://download.microsoft.com/download/F/9/1/F91B5312-4385-4476-9688-055E3B1ED10F/windowssdk/winsdksetup.exe" -OutFile "C:\tools\winsdksetup.exe" -UseBasicParsing
["C:\\tools\\winsdksetup.exe", "/q", "/norestart"]
 
# Download DotNet Core
Invoke-WebRequest "https://download.microsoft.com/download/E/7/8/E782433E-7737-4E6C-BFBF-290A0A81C3D7/dotnet-dev-win-x64.1.0.4.zip" -OutFile "C:\tools\dotnet-dev-win-x64.1.0.4.zip" -UseBasicParsing
Expand-Archive "C:\tools\dotnet-dev-win-x64.1.0.4.zip" -DestinationPath "C:\tools\dotnet"
 
# Download NuGet
Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "C:\tools\nuget.exe" -UseBasicParsing
 
# Download NUnit
Invoke-WebRequest "https://github.com/nunit/nunit-console/releases/download/3.6.1/NUnit.Console-3.6.1.zip" -OutFile "C:\tools\NUnit.Console-3.6.1.zip" -UseBasicParsing
Expand-Archive "C:\tools\NUnit.Console-3.6.1.zip" -DestinationPath "C:\tools\nunit"
 
# Install Web Targets
WORKDIR "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0"
 
["C:\\tools\\nuget.exe", "Install MSBuild.Microsoft.VisualStudio.Web.targets -Version 12.0.4"]
mv 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0\MSBuild.Microsoft.VisualStudio.Web.targets.12.0.4\tools\VSToolsPath\*' 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0\'
 
WORKDIR "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0"
 
["C:\\tools\\nuget.exe", "Install MSBuild.Microsoft.VisualStudio.Web.targets -Version 14.0.0.3"]
mv 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3\tools\VSToolsPath\*' 'C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\'
 
# Set paths
setx PATH '%PATH%;C:\tools;C:\tools\dotnet;C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\;C:\tools\nuget.exe'
setx MSBuildSDKsPath 'C:\tools\dotnet\sdk\1.0.4\Sdks'