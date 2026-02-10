# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY FraudRuleEngineService.sln .
COPY src/FraudEngine.Domain/FraudEngine.Domain.csproj src/FraudEngine.Domain/
COPY src/FraudEngine.Application/FraudEngine.Application.csproj src/FraudEngine.Application/
COPY src/FraudEngine.Infrastructure/FraudEngine.Infrastructure.csproj src/FraudEngine.Infrastructure/
COPY src/FraudEngine.API/FraudEngine.API.csproj src/FraudEngine.API/
COPY tests/FraudEngine.Domain.Tests/FraudEngine.Domain.Tests.csproj tests/FraudEngine.Domain.Tests/
COPY tests/FraudEngine.Application.Tests/FraudEngine.Application.Tests.csproj tests/FraudEngine.Application.Tests/
COPY tests/FraudEngine.API.Tests/FraudEngine.API.Tests.csproj tests/FraudEngine.API.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish src/FraudEngine.API/FraudEngine.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FraudEngine.API.dll"]
