# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/powershell:nanoserver-ltsc2022

# Force Docker to use PowerShell for subsequent RUN commands
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]

# Now you can run commands directly without prefixing "powershell"
RUN Install-PackageProvider -Name NuGet -Force

# The parent container needs the Docker CLI binary installed so it can issue commands like docker run.

WORKDIR C:/docker-cli

# Download and extract the official Windows Docker CLI binary
RUN powershell -Command \
    $ErrorActionPreference = 'Stop'; \
    Invoke-WebRequest -Uri https://docker.com -OutFile docker.zip; \
    Expand-Archive docker.zip -DestinationPath C:\; \
    Remove-Item docker.zip

# Add the binary path to the permanent System PATH
RUN setx /M PATH "%PATH%;C:\docker"

FROM mcr.microsoft.com/dotnet/runtime:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/", "ControllerConsoleApp_docker_sock/"]
RUN dotnet restore "./ControllerConsoleApp_docker_sock/ControllerConsoleApp_docker_sock.csproj"
COPY . .
WORKDIR "/src/ControllerConsoleApp_docker_sock"
RUN dotnet build "./ControllerConsoleApp_docker_sock.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ControllerConsoleApp_docker_sock.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControllerConsoleApp_docker_sock.dll"]