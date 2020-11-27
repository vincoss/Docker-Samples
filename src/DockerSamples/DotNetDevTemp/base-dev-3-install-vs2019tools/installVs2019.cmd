@ECHO ON

REM At first update the installer (both commands are required)
REM C:\temp\vs_2019_buildtools\offline\vs_setup.exe --update --quiet --wait

REM Now install|update build tools
REM C:\temp\vs_2019_buildtools\offline\vs_setup.exe --quiet --wait --all --norestart --nocache --installPath "C:\Vs2019BuildTools"

REM powershell Start-Process -Wait -FilePath "C:\Temp\vs_2019_buildtools\offline\vs_setup.exe" -ArgumentList "--quiet"

echo Begin installer update (both commands are required)
start /wait C:\temp\vs_2019_buildtools\offline\vs_setup.exe --update --quiet --wait
echo %errorlevel%

echo End of installer update

echo Begin install or update build tools
start /wait C:\temp\vs_2019_buildtools\offline\vs_setup.exe --quiet --passive --all --norestart --nocache --installPath "C:\Vs2019BuildTools"
echo %errorlevel%

echo End of tools install
