# RentIt - Next-Generation Modular Rental Platform

RentIt is a highly scalable, secure, and comprehensive rental platform tailored for the Ghanaian market. Built on modern .NET 10.0, the platform connects property hosts with renters through a seamlessly integrated ecosystem that handles everything from property discovery and identity verification to secure payment processing and post-stay reviews.

By leveraging **Clean Architecture**, **Domain-Driven Design (DDD)**, and a **Modular Monolith** approach, RentIt achieves the perfect balance between development velocity, operational simplicity, and future readiness for microservices extraction.

---

## âœ¨ Core Features

### For Renters
* **Smart Property Discovery:** Advanced search, filtering, and real-time availability checking.
* **Instant Bookings:** Frictionless booking workflows with real-time state management.
* **Secure Payments:** Integrated with Paystack for seamless local and international card processing, mobile money, and automated refunds.
* **Trust & Safety:** Identity verification workflows and encrypted PII storage.

### For Hosts
* **Property Management:** Rich dashboard to manage listings, dynamic pricing, and maintenance schedules.
* **Financial Analytics:** Real-time dashboards tracking estimated monthly earnings, occupancy rates, and payout history.
* **Automated Communications:** Trigger-based email and SMS notifications for booking confirmations and payment receipts.

---

## âš™ï¸  Architecture & Design Patterns

RentIt avoids the "distributed monolith" anti-pattern by starting as a **Modular Monolith**. The codebase is strictly partitioned into bounded contexts (Modules), communicating solely through asynchronous events or strictly defined in-memory contracts.

### Key Architectural Pillars
1. **Clean Architecture:** Each module is divided into `Domain`, `Application`, `Infrastructure`, and `Api` layers. Dependencies flow strictly inwards toward the Domain.
2. **Domain-Driven Design (DDD):** Rich domain models encapsulating business rules, utilizing Aggregates, Value Objects, and Domain Events.
3. **CQRS:** Segregation of commands (writes) and queries (reads) using **MediatR**, ensuring optimized database access patterns.
4. **Event-Driven Communication:** Cross-module communication is decoupled using **MassTransit**, paving the way for easy extraction into microservices in the future.
5. **Backend-For-Frontend (BFF):** Utilizing **YARP** as a reverse proxy/BFF and **Ocelot** as an API Gateway to aggregate module endpoints securely.

---

## ðŸ› ï¸  Technology Stack

### Backend & Infrastructure
* **Framework:** .NET 10.0 (C# 14)
* **Architecture:** ASP.NET Core Web API, Minimal APIs, YARP, Ocelot
* **Data Access:** Entity Framework Core (EF Core), PostgreSQL / SQL Server
* **Caching:** Redis (Distributed Caching)
* **Messaging:** MassTransit (w/ Amazon SQS or RabbitMQ)
* **Background Jobs:** Hangfire
* **CQRS & Mediator:** MediatR
* **Validation:** FluentValidation

### Security & Privacy
* **Encryption at Rest:** State-of-the-art **AES-GCM (256-bit)** encryption for Personally Identifiable Information (PII) and external provider tokens.
* **Authentication:** JWT Bearer tokens with strict role-based access control (RBAC).

### Utilities
* **Document Generation:** QuestPDF for dynamic invoice and receipt generation.
* **Email & Notifications:** FluentEmail.
* **Logging & Observability:** Serilog structured logging.

---

## ðŸ“‚ Solution Structure

```text
RentIt/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ Shared/                    # Cross-cutting concerns (Security, Logging, Exceptions)
â”‚   â”‚   â”œâ”€â”€ RentIt.Shared.Abstractions
â”‚   â”‚   â”œâ”€â”€ RentIt.Shared.Contracts
â”‚   â”‚   â”œâ”€â”€ RentIt.Shared.Kernel
â”‚   â”‚   â””â”€â”€ RentIt.Shared.Infrastructure
â”‚   â”‚
â”‚   â”œâ”€â”€ Modules/                   # Bounded Contexts
â”‚   â”‚   â”œâ”€â”€ Identity/              # Auth, Users, Roles, Profile Management
â”‚   â”‚   â”œâ”€â”€ Properties/            # Listings, Availability, Maintenance
â”‚   â”‚   â”œâ”€â”€ Payments/              # Webhooks, Invoices, Paystack Integration
â”‚   â”‚   â”œâ”€â”€ Bookings/              # Reservation logic, State machines
â”‚   â”‚   â”œâ”€â”€ Verification/          # KYC, Document Verification
â”‚   â”‚   â”œâ”€â”€ Messaging/             # In-app chat, SMS, Email
â”‚   â”‚   â”œâ”€â”€ Reviews/               # Ratings, Feedback loops
â”‚   â”‚   â””â”€â”€ Analytics/             # Host dashboards, Platform metrics
â”‚   â”‚
â”‚   â”œâ”€â”€ Host/                      # The Modular Monolith runtime host
â”‚   â”œâ”€â”€ ApiGateway/                # Ocelot Gateway routing
â”‚   â””â”€â”€ BFF/                       # Backend for Frontend orchestration
â”‚
â”œâ”€â”€ tests/                         # Unit, Integration, and Architecture tests
â””â”€â”€ docs/                          # Architecture Decision Records (ADRs) and API specs
```

---

## ðŸš€ Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL or SQL Server
- Redis (Local instance or Docker container)
- Docker Desktop (Optional, for simplified dependency spin-up)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/RentIt.git
   cd RentIt
   ```

2. **Configure AppSettings**
   Update the `appsettings.Development.json` in the `Host` project with your local database connection strings, Redis instance, and a valid 32-byte string for `AESGCM:KEY`.

3. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the Application**
   ```bash
   cd src/Host/RentIt.Host
   dotnet run
   ```

---

## ðŸ“ˆ Future Outlook & Roadmap

As the platform scales, the immediate roadmap includes:
1. **Microservices Transition:** Leveraging the existing MassTransit message bus to break out the `Analytics` and `Payments` modules into standalone microservices when load dictates.
2. **AI-Powered Recommendations:** Integrating machine learning models to suggest optimal pricing for hosts and personalized listings for renters.
3. **Advanced KYC:** Expanding the `Verification` module with automated OCR and biometric checks for frictionless host onboarding.

---

## ðŸ“„ License

Proprietary Software - All rights reserved.

## ðŸ“§ Contact

For technical inquiries, support, or partnership opportunities, please contact the lead architect:
**Email:** ziblimzulka.zz@gmail.com
