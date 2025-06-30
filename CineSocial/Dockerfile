# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies (leverage Docker layer caching)
COPY ["CineSocial.Adapters.WebAPI/CineSocial.Adapters.WebAPI.csproj", "CineSocial.Adapters.WebAPI/"]
COPY ["CineSocial.Core.Application/CineSocial.Core.Application.csproj", "CineSocial.Core.Application/"]
COPY ["CineSocial.Core.Domain/CineSocial.Core.Domain.csproj", "CineSocial.Core.Domain/"]
COPY ["CineSocial.Adapters.Infrastructure/CineSocial.Adapters.Infrastructure.csproj", "CineSocial.Adapters.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "CineSocial.Adapters.WebAPI/CineSocial.Adapters.WebAPI.csproj"

# Copy all source code
COPY . .

# Build and publish the application
WORKDIR "/src/CineSocial.Adapters.WebAPI"
RUN dotnet build "CineSocial.Adapters.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "CineSocial.Adapters.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=build /app/publish .

# Create directory for SQLite database with proper permissions
RUN mkdir -p /app/data && chown -R appuser:appuser /app/data

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/cinesocial.db;"

# Expose port
EXPOSE 5000

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "CineSocial.Adapters.WebAPI.dll"]