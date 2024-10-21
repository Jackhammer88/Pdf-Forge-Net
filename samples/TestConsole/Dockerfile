# Используем базовый образ Debian 12
FROM debian:12 AS base

# Обновляем пакеты и устанавливаем зависимости для .NET SDK
RUN apt-get update && apt-get install -y \
    bash \
    wget \
    gnupg \
    ca-certificates \
    apt-transport-https \
    && rm -rf /var/lib/apt/lists/*

# Добавляем Microsoft ключ и репозиторий для .NET SDK
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# Устанавливаем рабочую директорию для проекта
WORKDIR /app

# Копируем файлы проекта (например, csproj и sln)
COPY *.csproj ./

# Восстанавливаем зависимости проекта
RUN dotnet restore

# Копируем исходные файлы проекта в контейнер
COPY . .

# Сборка приложения в режиме Release
RUN dotnet build --configuration Release --no-restore

# Публикуем приложение
RUN dotnet publish --configuration Release --no-restore --output /app/publish

# Указываем команду по умолчанию для запуска приложения
ENTRYPOINT ["/bin/bash"]
