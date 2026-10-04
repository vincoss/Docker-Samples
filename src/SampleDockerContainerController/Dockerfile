# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./SampleDockerContainerController.csproj", "."]
RUN dotnet restore "./SampleDockerContainerController.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./SampleDockerContainerController.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SampleDockerContainerController.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleDockerContainerController.dll"]

# docker build -f Dockerfile --no-cache -t sampledockercontainercontroller .
# docker run -it --rm -p 5000:8080 -v /var/run/docker.sock:/var/run/docker.sock --name sampledockercontainercontroller sampledockercontainercontroller