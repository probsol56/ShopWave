# ShopWave — Multi-tenant Shop & Inventory API

A production-ready **multi-tenant SaaS backend** for managing shops, products, stock, and sales orders. Built with Clean Architecture and CQRS on .NET 10.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-336791?logo=postgresql)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Features

- **Multi-tenancy** — Shared database with EF Core Global Query Filters. Every query is automatically scoped to the authenticated tenant.
- **Clean Architecture** — Domain → Application → Infrastructure → API. Zero framework leakage into business logic.
- **CQRS with MediatR** — Commands and queries are fully separated with a validation pipeline via FluentValidation.
- **JWT Auth** — Access token + refresh token flow. Role-based access (Owner / Manager / Staff).
- **Inventory tracking** — Stock In / Out / Adjustment entries with low-stock alerts.
- **Sales orders** — Create orders, auto-deduct stock, update order status.
- **Dashboard** — Today's sales, total products, low stock count, recent orders.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 10 |
| Framework | ASP.NET Core 10 |
| ORM | EF Core 10 + Npgsql |
| Database | PostgreSQL |
| Auth | JWT Bearer + Refresh Tokens |
| CQRS | MediatR 14 |
| Validation | FluentValidation 12 |
| Logging | Serilog |
| API Docs | Scalar (OpenAPI) |
| Password | BCrypt.Net-Next |

---

## Architecture

```
ShopWave.sln
├── src/
│   ├── ShopWave.Domain/          # Entities, Enums, Base classes
│   ├── ShopWave.Application/     # CQRS Handlers, Interfaces, Behaviors
│   ├── ShopWave.Infrastructure/  # EF Core, JWT, Services
│   └── ShopWave.API/             # Controllers, Middleware, Program.cs
└── tests/
    └── ShopWave.UnitTests/       # Validator & handler unit tests
```

---

## API Endpoints

```
# Auth
POST   /api/auth/register          Create tenant + owner account
POST   /api/auth/login
POST   /api/auth/refresh

# Tenant
GET    /api/tenant/me
PUT    /api/tenant/me

# Shops
GET    /api/shops
POST   /api/shops
GET    /api/shops/{id}
PUT    /api/shops/{id}
DELETE /api/shops/{id}

# Categories
GET    /api/shops/{shopId}/categories
POST   /api/shops/{shopId}/categories
PUT    /api/shops/{shopId}/categories/{id}

# Products
GET    /api/shops/{shopId}/products
POST   /api/shops/{shopId}/products
GET    /api/shops/{shopId}/products/{id}
PUT    /api/shops/{shopId}/products/{id}
DELETE /api/shops/{shopId}/products/{id}

# Stock
GET    /api/shops/{shopId}/stock
GET    /api/shops/{shopId}/stock/low-stock
POST   /api/shops/{shopId}/stock/adjust

# Sales Orders
GET    /api/shops/{shopId}/sale-orders
POST   /api/shops/{shopId}/sale-orders
GET    /api/shops/{shopId}/sale-orders/{id}
PUT    /api/shops/{shopId}/sale-orders/{id}/status

# Dashboard
GET    /api/shops/{shopId}/dashboard
```

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

### 1. Clone & configure

```bash
git clone https://github.com/your-username/shopwave.git
cd shopwave
```

Update `src/ShopWave.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=shopwave_dev;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Secret": "your-secret-key-min-32-characters-long"
  }
}
```

### 2. Run

```bash
dotnet run --project src/ShopWave.API
```

The database is **auto-migrated on startup**. API docs available at:

```
https://localhost:{port}/scalar/v1
```

### 3. Test (register first)

```bash
curl -X POST https://localhost:{port}/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "tenantName": "Acme Store",
    "tenantSlug": "acme-store",
    "email": "owner@acme.com",
    "password": "SecurePass1!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

Use the returned `accessToken` as `Bearer` token for all subsequent requests.

---

## Multi-tenancy Design

Every entity implements `ITenantEntity` and carries a `TenantId` column. EF Core **Global Query Filters** automatically append `WHERE TenantId = @current` to every query — no manual filtering needed in handlers.

The `TenantResolutionMiddleware` extracts `TenantId` from the JWT claim on every request and injects it into the scoped `ITenantContext`. This makes tenant isolation automatic and impossible to accidentally bypass in normal usage.

---

## Running Tests

```bash
dotnet test
```

---

## Deployment

Set the following environment variables on your host (Railway, Render, etc.):

```
ConnectionStrings__DefaultConnection=Host=...;Database=shopwave;Username=...;Password=...
Jwt__Secret=<strong-random-secret>
Jwt__Issuer=ShopWave
Jwt__Audience=ShopWave
AutoMigrate=true
```

---

## Roadmap (v1.1)

- [ ] React frontend dashboard
- [ ] Supplier management
- [ ] Purchase orders
- [ ] Product variants
- [ ] Detailed sales reports

---

## License

MIT
