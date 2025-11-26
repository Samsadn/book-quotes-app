# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy entire repo
COPY . .

# Go into Backend project folder
WORKDIR /src/Backend

# Restore dependencies
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Backend.dll", "--urls", "http://0.0.0.0:5000"]
