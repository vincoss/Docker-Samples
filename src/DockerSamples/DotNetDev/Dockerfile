# escape=`

FROM vincoss/dotnetdev:baseinstallsVsToolsCopy AS Basefile

# Copy build targets
#COPY ["./vs_2019_buildtools", "C:/Temp/vs_2019_buildtools/"]

#RUN C:\TEMP\vs_2019_buildtools\vs_setup.exe --all --quiet --wait --norestart --nocache `
    #--installPath C:\Vs2019BuildTools

# Restore the default Windows shell for correct batch processing.
SHELL ["cmd", "/S", "/C"]

RUN C:\TEMP\vs_2019_buildtools\offline\vs_setup.exe --all --quiet --wait --norestart --nocache `
    --installPath C:\Vs2019BuildTools

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]