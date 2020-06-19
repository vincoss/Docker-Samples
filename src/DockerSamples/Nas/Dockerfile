# base image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

ARG HTTP_PORT=5000
ARG APP_HOME=/app

WORKDIR ${APP_HOME}
COPY ["/win-x64/.", "./"]

ENV HOME ${APP_HOME}
ENV NAME nassample

# Make ports available to the world outside this container for main web interface
EXPOSE ${HTTP_PORT}

#VOLUME C:/Temp/Docker/Volume

ENTRYPOINT ["dotnet", "Docker.TesterWebSite.dll"]