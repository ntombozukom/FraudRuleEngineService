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

SQL Server runs on port **1434**. See `.env.example` for credential setup.

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

## API Endpoints

### Transaction Evaluation

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/transactions/evaluate` | Submit a transaction for fraud evaluation |

### Fraud Alerts

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/fraud-alerts` | List alerts with filtering and pagination |
| `GET` | `/api/fraud-alerts/{id}` | Get a single alert by ID |
| `PATCH` | `/api/fraud-alerts/{id}/review` | Review or dismiss an alert |
| `GET` | `/api/fraud-alerts/statistics` | Aggregate alert statistics |

### Fraud Rules Configuration

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/fraud-rules` | List all configured rules |
| `PUT` | `/api/fraud-rules/{ruleName}` | Update rule config (enable/disable, parameters) |

### Query Parameters (GET /api/fraud-alerts)

| Parameter | Type | Description |
|-----------|------|-------------|
| `accountNumber` | string | Filter by account number (10-13 digits) |
| `status` | string | Filter by status (Open, UnderReview, Reviewed, Dismissed) |
| `severity` | string | Filter by severity (Low, Medium, High, Critical) |
| `from` | DateTime | Start date filter |
| `to` | DateTime | End date filter |
| `ruleName` | string | Filter by rule name |
| `page` | int | Page number (default: 1) |
| `pageSize` | int | Items per page (default: 20) |

## Fraud Rules

Five configurable fraud rules are implemented, each as a separate strategy class:

| Rule | Default Config | Severity | Description |
|------|---------------|----------|-------------|
| **HighValueTransaction** | Threshold: R50,000 | High | Flags transactions exceeding the amount threshold |
| **RapidSuccessiveTransactions** | 3 txns in 5 min | Medium | Flags multiple transactions in a short window |
| **CrossBorderTransaction** | Home: ZA | Medium | Flags transactions from outside South Africa |
| **AfterHoursTransaction** | 23:00-05:00 | Low | Flags transactions during unusual hours |
| **UnusualCategory** | <5% of history | Low | Flags spending in uncommon categories for the account |

### Configuring Rules

Rules can be enabled/disabled and their parameters updated at runtime:

```bash
# Disable a rule
curl -X PUT http://localhost:5001/api/fraud-rules/AfterHoursTransaction \
  -H "Content-Type: application/json" \
  -d '{"isEnabled": false}'

# Update threshold
curl -X PUT http://localhost:5001/api/fraud-rules/HighValueTransaction \
  -H "Content-Type: application/json" \
  -d '{"parameters": "{\"ThresholdAmount\": 100000}"}'
```

## Example Usage

### Evaluate a Transaction

```bash
curl -X POST http://localhost:5001/api/transactions/evaluate \
  -H "Content-Type: application/json" \
  -d '{
    "accountNumber": "1234567890",
    "amount": 75000,
    "currency": "ZAR",
    "merchantName": "Luxury Jewellers",
    "category": "Shopping",
    "transactionDate": "2025-01-15T14:30:00",
    "location": "Sandton City",
    "country": "ZA",
    "channel": 1
  }'
```

Response:
```json
{
  "transactionId": "...",
  "isFlagged": true,
  "alertsGenerated": 1,
  "alerts": [
    {
      "ruleName": "HighValueTransaction",
      "severity": "High",
      "description": "Transaction amount R75,000.00 exceeds threshold of R50,000.00."
    }
  ]
}
```

### Alert Lifecycle

1. Transaction is evaluated → alerts created with `Open` status
2. Analyst reviews → `PATCH /api/fraud-alerts/{id}/review` with `UnderReview`
3. Analyst completes review → `PATCH` with `Reviewed` or `Dismissed`
4. Full audit trail: `ReviewedBy` and `ReviewedAt` are recorded

## Running Tests

```bash
dotnet test
```

This runs all three test projects:

- **Domain.Tests** — Entity and enum tests
- **Application.Tests** — Fraud rule logic tests (HighValue, AfterHours, CrossBorder) using Moq
- **API.Tests** — Integration tests using `WebApplicationFactory` with in-memory database

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
