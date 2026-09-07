
# Readme

``` Build
docker build -f Dockerfile --no-cache -t vincoss/default_webapplication1:1.0.0-windows .
```

### Run
```
docker run -it --rm -p 11234:8080 --name default_webapplication1-windows vincoss/default_webapplication1:1.0.0-windows
```

### Compose
```
docker compose -f docker-compose.yml up -d -no-cache
```

### SSL - Export Your Dev Certificate
```
dotnet dev-certs https --trust
dotnet dev-certs https -ep "$env:USERPROFILE\$env:USERPROFILE\AppData\Roaming\ASP.NET\https\Default_WebApplication1.pfx" -p "Pass@word"
```

### Browse
http://localhost:11234/WeatherForecast
https://localhost:11235/WeatherForecast