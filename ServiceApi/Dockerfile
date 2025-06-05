# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["./ServiceApi.csproj", "./"]
Run dotnet restore "./ServiceApi.csproj"
COPY . .
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:80"]

