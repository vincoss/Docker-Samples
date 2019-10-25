# escape=`

FROM vincoss/dotnetdev:base-dev-1-install-tools AS base-dev-2-copy-vs2019tools

# Copy build targets
COPY ["./vs_2019_buildtools", "C:/Temp/vs_2019_buildtools/"]

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create images
# docker build --no-cache --force-rm -t vincoss/dotnetdev:base-dev-2-copy-vs2019tools .

# Run image
#docker container run vincoss/dotnetdev:base-dev-2-copy-vs2019tools
#docker run --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-2-copy-vs2019tools
#docker run --net=isolated_nw --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-2-copy-vs2019tools

#size
#docker run --storage-opt "size=100GB" --name TeamCityAgent0001 -h TeamCityAgent0001 -it -p 8000:80 vincoss/dotnetdev:base-dev-2-copy-vs2019tools