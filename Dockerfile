# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файлы проекта
COPY ["ClipBoardHelper.csproj", "."]
RUN dotnet restore "ClipBoardHelper.csproj"
COPY . .

# Собираем приложение
RUN dotnet publish "ClipBoardHelper.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Указываем порт, который Render использует
EXPOSE 80

# Запускаем приложение
ENTRYPOINT ["dotnet", "ClipBoardHelper.dll"]