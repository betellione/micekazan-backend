﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Micekazan.PrintDispatcher/Micekazan.PrintDispatcher.csproj Micekazan.PrintDispatcher/
COPY Micekazan.PrintDispatcher.Domain/Micekazan.PrintDispatcher.Domain.csproj Micekazan.PrintDispatcher.Domain/

COPY Micekazan.PrintDispatcher/ Micekazan.PrintDispatcher/
COPY Micekazan.PrintDispatcher.Domain/ Micekazan.PrintDispatcher.Domain/

RUN dotnet restore Micekazan.PrintDispatcher/Micekazan.PrintDispatcher.csproj
WORKDIR /src/Micekazan.PrintDispatcher
RUN dotnet build Micekazan.PrintDispatcher.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Micekazan.PrintDispatcher.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Micekazan.PrintDispatcher.dll