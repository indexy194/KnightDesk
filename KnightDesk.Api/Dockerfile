# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["KnightDesk.Api/KnightDesk.Api.csproj", "KnightDesk.Api/"]
COPY ["KnightDesk.Core/KnightDesk.Core.csproj", "KnightDesk.Core/"]
COPY ["KnightDesk.Infrastructure/KnightDesk.Infrastructure.csproj", "KnightDesk.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "KnightDesk.Api/KnightDesk.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/KnightDesk.Api"
RUN dotnet build "KnightDesk.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "KnightDesk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "KnightDesk.Api.dll"]
