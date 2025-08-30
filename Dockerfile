FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
WORKDIR /app/src/Sefer.Backend.Api
RUN dotnet restore 
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
RUN apk add --no-cache icu-libs
WORKDIR /app
COPY --from=build /app/published-app /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
ENTRYPOINT [ "dotnet", "/app/Sefer.Backend.Api.dll" ]