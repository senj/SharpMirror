## Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Change directory and copy all files
WORKDIR /src
COPY Source/ .

# Restore solution packages
RUN dotnet restore

# Build API-Service
RUN dotnet build SmartMirror/SmartMirror.csproj -c Release -o /app --no-restore

# Publish API-Service
RUN dotnet publish SmartMirror/SmartMirror.csproj -c Release -o /app --no-restore -r linux-arm

## Base
# 3.1.4-buster-slim-arm32v7
FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim-arm32v7 AS base
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "SmartMirror.dll"]