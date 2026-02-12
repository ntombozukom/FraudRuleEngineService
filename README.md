# Fraud Rule Engine Service

ASP.NET Core 8 Minimal API that processes categorized transaction events, evaluates them against configurable fraud rules, and flags potential fraud. Rules can be enabled/disabled and their parameters tuned at runtime via API.

## Architecture

This project follows **minimal API** with four layers:

```
FraudEngine.Domain          → Entities, interfaces, enums (no dependencies)
FraudEngine.Application     → Use cases, MediatR handlers, DTOs, validation
FraudEngine.Infrastructure  → EF Core, repositories, fraud rule implementations, services
FraudEngine.API             → Minimal API endpoints, middleware, composition root
```

Dependencies point inward only — the Domain layer has zero external dependencies.

### Key Design Patterns

- **Minimal API** — Lightweight endpoint definitions using `MapGroup` and `MapGet`/`MapPost`/`MapPut`/`MapPatch` with dependency injection via lambda parameters
- **Strategy Pattern** — Each fraud rule implements `IFraudRule` and is independently configurable
- **CQRS** via MediatR — Commands (evaluate transaction, review alert) and queries are separate
- **Repository + Unit of Work** — Abstracts data access behind interfaces
- **Pipeline Behavior** — FluentValidation integrated into MediatR pipeline

### Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core 8 |
| API Style | Minimal API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| Mapping | Manual (compile-time safe extension methods) |
| Caching | IMemoryCache (30-min TTL for fraud rules) |
| Logging | Serilog (structured, console + file) |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit, Moq, FluentAssertions |
| Containerisation | Docker + Docker Compose |

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) (Docker Desktop recommended)
- OR [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) + SQL Server for local development

## Getting Started

### Option 1: Docker (Recommended)

```bash
git clone <repository-url>
cd FraudRuleEngineService

docker-compose up --build
```

The API will be available at **http://localhost:5001** with Swagger UI at the root.

#### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `SA_PASSWORD` | SQL Server password | Set in docker-compose.yml |
| `ConnectionStrings__DefaultConnection` | Database connection string | Set in docker-compose.yml |

### Option 2: Local Development

1. Ensure SQL Server is running locally on port 1434
2. Update the connection string in `src/FraudEngine.API/appsettings.Development.json` if needed
3. Run:

```bash
dotnet restore
dotnet run --project src/FraudEngine.API
```

The API will be available at **https://localhost:7171** (or http://localhost:5025) with Swagger UI at the root.

## Database Setup

The database is created automatically on startup via EF Core migrations. Five default fraud rules are seeded with their configurations.


## Project Structure

```
FraudRuleEngineService/
├── src/
│   ├── FraudEngine.Domain/
│   │   ├── Common/              # Shared types (PagedResponse)
│   │   ├── Entities/            # Transaction, FraudAlert, FraudRuleConfiguration
│   │   ├── Enums/               # AlertSeverity, AlertStatus, TransactionChannel
│   │   └── Interfaces/          # IFraudRule, repositories, IUnitOfWork
│   ├── FraudEngine.Application/
│   │   ├── Behaviors/           # MediatR pipeline (validation)
│   │   ├── DTOs/                # Data transfer objects
│   │   ├── Features/            # CQRS handlers (queries & commands)
│   │   ├── Mappings/            # Manual mapping extensions (one per entity)
│   │   │   ├── TransactionMappingExtensions.cs
│   │   │   ├── FraudAlertMappingExtensions.cs
│   │   │   └── FraudRuleMappingExtensions.cs
│   │   └── Services/            # IFraudRuleEngine, IFraudRuleConfigurationCache
│   ├── FraudEngine.Infrastructure/
│   │   ├── Caching/             # FraudRuleConfigurationCache (IMemoryCache)
│   │   ├── FraudRules/          # 5 IFraudRule strategy implementations
│   │   ├── Persistence/         # DbContext, configurations, repositories
│   │   └── Services/            # FraudRuleEngine orchestrator
│   └── FraudEngine.API/
│       ├── Constants/           # Tags, Routes, ConfigurationKeys, ConfigurationDefaults
│       ├── Endpoints/           # Vertical slice (one endpoint per file)
│       │   ├── FraudAlerts/
│       │   │   ├── GetFraudAlerts.cs
│       │   │   ├── GetFraudAlertByReference.cs
│       │   │   ├── ReviewFraudAlert.cs
│       │   │   └── GetFraudAlertStatistics.cs
│       │   ├── FraudRules/
│       │   │   ├── GetFraudRules.cs
│       │   │   └── UpdateFraudRule.cs
│       │   └── Transactions/
│       │       └── EvaluateTransaction.cs
│       ├── Extensions/          # DI and middleware configuration
│       └── Middleware/          # Exception handling
├── tests/
│   ├── FraudEngine.Domain.Tests/
│   ├── FraudEngine.Application.Tests/
│   └── FraudEngine.API.Tests/
├── docker-compose.yml
├── Dockerfile
└── README.md
```
