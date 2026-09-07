# 1. Choose the base runtime image for the final lightweight container
FROM mcr.microsoft.com/dotnet/runtime:10.0-nanoserver-ltsc2025 AS base
WORKDIR /app

# 2. Use the full SDK image to compile and build the solution
FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
WORKDIR /src

# 3. Copy solution and source folder
COPY ["Docker-Samples.sln", "./"]
COPY ["src/.", "./src/"]

RUN DIR

# 4. Restore packages across the whole solution
RUN dotnet restore "Docker-Samples.sln"

# 5. Copy the entire source tree now that dependencies are cached
COPY . .

# 6. Publish the compiled binaries into a clean directory
FROM build AS publish
RUN dotnet publish "Docker-Samples.sln" -c Release -p:PublishDir=./bin/release/publish/ /p:UseAppHost=false

RUN DIR

# 6. Final Stage: Build runtime container using only the compiled assets
FROM base AS final
WORKDIR /app
COPY --from=publish /src/src/ConsoleApp1/bin/Release/publish/ ./ConsoleApp1/

CMD ["cmd.exe"]
#CMD ["ls", "-R"]

### docker build -f Dockerfile --no-cache -t whole-solution-app .
### docker run -it --name whole-solution-app whole-solution-app