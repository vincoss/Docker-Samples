# base image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

ARG HTTP_PORT=80
ARG APP_HOME=/app

WORKDIR ${APP_HOME}
COPY ["/win-x64/.", "./"]

ENV HOME ${APP_HOME}
ENV NAME smb-sample

# Make ports available to the world outside this container for main web interface
EXPOSE ${HTTP_PORT}

VOLUME C:/appdata

ENTRYPOINT ["dotnet", "Docker.TesterWebSite.dll"]
