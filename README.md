# RentIt - Modular Monolith with Clean Architecture

A rental platform for Ghana built with ASP.NET Core, implementing Clean Architecture and Domain-Driven Design principles in a modular monolith architecture.

## Solution Structure

```
RentIt/
├── src/
│   ├── Shared/                    # Shared libraries
│   │   ├── RentIt.Shared.Abstractions
│   │   ├── RentIt.Shared.Contracts
│   │   ├── RentIt.Shared.Kernel
│   │   └── RentIt.Shared.Infrastructure
│   │
│   ├── Modules/                   # Business modules
│   │   ├── Identity/
│   │   ├── Properties/
│   │   ├── Payments/
│   │   ├── Bookings/
│   │   ├── Verification/
│   │   ├── Messaging/
│   │   ├── Reviews/
│   │   └── Analytics/
│   │
│   ├── Host/                      # Main application host
│   ├── ApiGateway/                # Ocelot API Gateway
│   └── BFF/                       # Backend for Frontend (YARP)
│
├── tests/                         # Test projects
└── docs/                          # Documentation
```

## Technology Stack

- **.NET 10.0**
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **MassTransit + Amazon SQS** - Message broker
- **Redis** - Caching
- **Hangfire** - Background jobs
- **Ocelot** - API Gateway
- **YARP** - Reverse proxy for BFF
- **Paystack.Net** - Payment gateway
- **QuestPDF** - PDF generation
- **FluentEmail** - Email service

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/) or [SQL Server](https://www.microsoft.com/sql-server)
- [Redis](https://redis.io/)
- [Docker](https://www.docker.com/) (optional, for local development)

### Building the Solution

```bash
dotnet restore
dotnet build
```

### Running the Application

```bash
cd src/Host/RentIt.Host
dotnet run
```

## Module Architecture

Each module follows Clean Architecture with 4 layers:

1. **Domain Layer** - Entities, value objects, domain events, repository interfaces
2. **Application Layer** - Use cases (commands/queries), DTOs, handlers, validators
3. **Infrastructure Layer** - Repository implementations, EF Core configurations, external services
4. **API Layer** - Controllers, endpoints, filters, middleware

## Documentation

See the [docs](./docs) folder for:
- Architecture decisions
- API documentation
- Deployment guides

## License

Proprietary - All rights reserved

## Contact

For questions or support, contact: [Your Contact Information]
