﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebApp1/WebApp1.csproj WebApp1/
COPY Micekazan.Infrastructure/Micekazan.Infrastructure.csproj Micekazan.Infrastructure/

COPY WebApp1/ WebApp1/
COPY Micekazan.Infrastructure/ Micekazan.Infrastructure/

RUN dotnet restore WebApp1/WebApp1.csproj
WORKDIR /src/WebApp1
RUN dotnet build WebApp1.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish WebApp1.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet WebApp1.dll