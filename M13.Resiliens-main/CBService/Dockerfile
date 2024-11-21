# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish /app
ENTRYPOINT ["dotnet", "CBService.dll"]