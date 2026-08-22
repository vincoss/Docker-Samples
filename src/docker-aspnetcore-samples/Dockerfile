# Step 1: Use the SDK to build the code
FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
WORKDIR /src

# Copy all project files to restore them first
COPY Docker-Net-Samples.sln ./
COPY Default_AspNetCoreWebApi/Default_AspNetCoreWebApi.csproj StoreApi/
COPY Default_AspNetCoreWebApiSdk/Default_AspNetCoreWebApiSdk.csproj StoreLogic/
RUN dotnet restore Docker-Net-Samples.sln

# Copy the rest of the code and build it
COPY . .
WORKDIR /src/Default_AspNetCoreWebApi
RUN dotnet publish -c Release -o /app/out

# Step 2: Use a small runtime to run the app
FROM mcr.microsoft.com/dotnet/runtime:10.0-nanoserver-ltsc2022 AS final
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Default_AspNetCoreWebApi.dll"]
