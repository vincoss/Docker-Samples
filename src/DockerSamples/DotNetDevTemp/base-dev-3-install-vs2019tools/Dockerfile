#escape=`

FROM vincoss/dotnetdev:base-dev-2-copy-vs2019tools AS base-dev-3-install-vs2019tools

#VOLUME C:/Glut/Data
#VOLUME C:/Users
#RUN powershell [Environment]::UserName

COPY installVs2019.cmd C:/temp/
COPY install.cmd C:\temp\

# Download collect.exe in case of an install failure.
ADD https://aka.ms/vscollect.exe C:\TEMP\collect.exe

# Restore the default Windows shell for correct batch processing.
SHELL ["cmd", "/S", "/C"]

#RUN .\temp\install.cmd
#RUN .\temp\installVs2019.cmd

#RUN C:\temp\vs_2019_buildtools\offline\vs_setup.exe --update --quiet --wait
#
#RUN C:\temp\vs_2019_buildtools\offline\vs_setup.exe --quiet --passive --norestart --nocache `
    #--installPath C:\Vs2019BuildTools `
    #--all

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create images
# docker build --no-cache --force-rm -t vincoss/dotnetdev:base-dev-3-install-vs2019tools .

# Run image
#docker container run vincoss/dotnetdev:base-dev-3-install-vs2019tools
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-3-install-vs2019tools
#docker run --net=isolated_nw --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-3-install-vs2019tools
# Volume
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 -v vsdata:C:/Users vincoss/dotnetdev:base-dev-3-install-vs2019tools
#size
#docker run --storage-opt "size=100GB" --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-3-install-vs2019tools