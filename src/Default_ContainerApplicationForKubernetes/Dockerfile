FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["src/Default_ContainerApplicationForKubernetes/Default_ContainerApplicationForKubernetes.csproj", "src/Default_ContainerApplicationForKubernetes/"]

RUN dotnet restore "src/Default_ContainerApplicationForKubernetes/Default_ContainerApplicationForKubernetes.csproj"
COPY . .
WORKDIR "/src/src/Default_ContainerApplicationForKubernetes"
RUN dotnet build "Default_ContainerApplicationForKubernetes.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Default_ContainerApplicationForKubernetes.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Default_ContainerApplicationForKubernetes.dll"]