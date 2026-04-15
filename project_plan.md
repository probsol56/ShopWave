# 📦 Multi-tenant Inventory / Shop Management API — Project Plan

> **Stack:** ASP.NET Core 8, EF Core, PostgreSQL, JWT Auth, Clean Architecture  
> **Pattern:** SaaS Multi-tenancy (Tenant-per-schema or shared DB with TenantId column)

---

## 🏗️ Architecture Overview

```
ShopMgmt.sln
├── src/
│   ├── ShopMgmt.Domain/           # Entities, Value Objects, Domain Events
│   ├── ShopMgmt.Application/      # Use Cases (CQRS), DTOs, Interfaces
│   ├── ShopMgmt.Infrastructure/   # EF Core, Repos, JWT, Email Services
│   └── ShopMgmt.API/              # Controllers, Middleware, Program.cs
└── tests/
    ├── ShopMgmt.UnitTests/
    └── ShopMgmt.IntegrationTests/
```

---

## 🧩 Domain Model

### Core Entities

| Entity | Fields |
|--------|--------|
| `Tenant` | Id, Name, Slug, Plan (Free/Pro), IsActive, CreatedAt |
| `User` | Id, TenantId, Email, PasswordHash, Role (Owner/Manager/Staff) |
| `Shop` | Id, TenantId, Name, Address, Phone, Currency, IsActive |
| `Category` | Id, TenantId, ShopId, Name, ParentCategoryId |
| `Product` | Id, TenantId, ShopId, CategoryId, Name, SKU, Description, ImageUrl |
| `ProductVariant` | Id, ProductId, Name (Size/Color), Price, CostPrice |
| `StockEntry` | Id, TenantId, ShopId, ProductVariantId, Quantity, Type (In/Out/Adjustment) |
| `Supplier` | Id, TenantId, Name, Phone, Email, Address |
| `PurchaseOrder` | Id, TenantId, ShopId, SupplierId, Status, TotalAmount |
| `PurchaseOrderItem` | Id, PurchaseOrderId, ProductVariantId, Quantity, UnitCost |
| `SaleOrder` | Id, TenantId, ShopId, CustomerName, CustomerPhone, Status, TotalAmount |
| `SaleOrderItem` | Id, SaleOrderId, ProductVariantId, Quantity, UnitPrice |

---

## 🔐 Multi-Tenancy Strategy

**Shared Database, Tenant-Filtered Rows**
- Every entity has a `TenantId` (Guid)
- A custom `ITenantContext` interface resolves the current tenant from JWT claims
- A global EF Core query filter applies `WHERE TenantId = @currentTenant` automatically
- Middleware validates the tenant on every request

---

## 🛣️ API Endpoints

### Auth
```
POST /api/auth/register          # Register new tenant + owner account
POST /api/auth/login             # Login, returns JWT
POST /api/auth/refresh           # Refresh token
```

### Tenant Management
```
GET  /api/tenant/me              # Get current tenant info
PUT  /api/tenant/me              # Update tenant profile
```

### Shops
```
GET    /api/shops                # List all shops in tenant
POST   /api/shops                # Create a new shop
GET    /api/shops/{id}           # Get shop detail
PUT    /api/shops/{id}           # Update shop
DELETE /api/shops/{id}           # Soft-delete shop
```

### Products & Inventory
```
GET    /api/shops/{shopId}/products
POST   /api/shops/{shopId}/products
GET    /api/shops/{shopId}/products/{id}
PUT    /api/shops/{shopId}/products/{id}
DELETE /api/shops/{shopId}/products/{id}

GET    /api/shops/{shopId}/products/{id}/variants
POST   /api/shops/{shopId}/products/{id}/variants

# Stock
GET    /api/shops/{shopId}/stock                   # Current stock levels
POST   /api/shops/{shopId}/stock/adjust            # Stock adjustment
GET    /api/shops/{shopId}/stock/low-stock         # Low stock alerts
```

### Categories
```
GET  /api/shops/{shopId}/categories
POST /api/shops/{shopId}/categories
PUT  /api/shops/{shopId}/categories/{id}
```

### Suppliers
```
GET    /api/suppliers
POST   /api/suppliers
PUT    /api/suppliers/{id}
DELETE /api/suppliers/{id}
```

### Purchase Orders
```
GET    /api/shops/{shopId}/purchase-orders
POST   /api/shops/{shopId}/purchase-orders
GET    /api/shops/{shopId}/purchase-orders/{id}
PUT    /api/shops/{shopId}/purchase-orders/{id}/status
```

### Sales Orders
```
GET    /api/shops/{shopId}/sale-orders
POST   /api/shops/{shopId}/sale-orders
GET    /api/shops/{shopId}/sale-orders/{id}
PUT    /api/shops/{shopId}/sale-orders/{id}/status
```

### Dashboard / Reports
```
GET /api/shops/{shopId}/dashboard    # Summary stats
GET /api/shops/{shopId}/reports/sales?from=&to=
GET /api/shops/{shopId}/reports/inventory
```

---

## 📦 NuGet Packages

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` | ORM |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | PostgreSQL provider |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Auth |
| `MediatR` | CQRS pattern |
| `FluentValidation.AspNetCore` | Request validation |
| `Mapster` | DTO mapping |
| `Serilog.AspNetCore` | Structured logging |
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI |
| `BCrypt.Net-Next` | Password hashing |
| `CSharpFunctionalExtensions` | Result pattern (optional) |

---

## 🗂️ Folder Structure Detail

```
ShopMgmt.Domain/
├── Common/
│   ├── BaseEntity.cs
│   ├── AuditableEntity.cs
│   └── ITenantEntity.cs
├── Entities/
│   ├── Tenant.cs
│   ├── User.cs
│   ├── Shop.cs
│   ├── Product.cs
│   ├── ProductVariant.cs
│   ├── Category.cs
│   ├── StockEntry.cs
│   ├── Supplier.cs
│   ├── PurchaseOrder.cs
│   ├── PurchaseOrderItem.cs
│   ├── SaleOrder.cs
│   └── SaleOrderItem.cs
├── Enums/
│   ├── UserRole.cs
│   ├── PlanType.cs
│   ├── StockEntryType.cs
│   └── OrderStatus.cs
└── Events/
    ├── StockLowEvent.cs
    └── OrderPlacedEvent.cs

ShopMgmt.Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IAppDbContext.cs
│   │   ├── ITenantContext.cs
│   │   └── ICurrentUserService.cs
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs
│   │   └── LoggingBehavior.cs
│   └── Exceptions/
│       ├── NotFoundException.cs
│       ├── UnauthorizedException.cs
│       └── ValidationException.cs
├── Features/
│   ├── Auth/
│   │   ├── Commands/RegisterTenant/
│   │   └── Commands/Login/
│   ├── Shops/
│   │   ├── Commands/ (Create, Update, Delete)
│   │   └── Queries/  (GetAll, GetById)
│   ├── Products/...
│   ├── Stock/...
│   ├── Suppliers/...
│   ├── PurchaseOrders/...
│   ├── SaleOrders/...
│   └── Dashboard/...
└── DTOs/
    ├── ShopDto.cs
    ├── ProductDto.cs
    └── ...

ShopMgmt.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/ (IEntityTypeConfiguration)
│   ├── Repositories/
│   └── Migrations/
├── Identity/
│   ├── JwtTokenService.cs
│   └── PasswordHasher.cs
└── Services/
    ├── TenantContext.cs
    └── CurrentUserService.cs

ShopMgmt.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── ShopsController.cs
│   ├── ProductsController.cs
│   ├── StockController.cs
│   ├── SuppliersController.cs
│   ├── PurchaseOrdersController.cs
│   ├── SaleOrdersController.cs
│   └── DashboardController.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── TenantResolutionMiddleware.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

---

## 🚀 Implementation Order

1. **Solution & Project Setup** — Create sln, 4 projects, add references + packages
2. **Domain Layer** — All entities, enums, base classes
3. **Infrastructure: DbContext** — EF config, global query filters for tenancy
4. **Application: Auth** — Register tenant, login, JWT generation
5. **Application: Shops** — CRUD with CQRS
6. **Application: Products + Variants** 
7. **Application: Stock Management**
8. **Application: Suppliers + Purchase Orders**
9. **Application: Sale Orders**
10. **Application: Dashboard / Reports**
11. **API Controllers + Middleware**
12. **Swagger + Seed Data**
13. **README + Docker Compose**

---

## 📋 README Highlights (for GitHub)

- ✅ Multi-tenant SaaS architecture with JWT-based tenant isolation
- ✅ Clean Architecture (Domain / Application / Infrastructure / API)
- ✅ CQRS with MediatR + FluentValidation pipeline behaviors
- ✅ EF Core global query filters for automatic tenant scoping
- ✅ Role-based access (Owner / Manager / Staff)
- ✅ Real-time low-stock alerts (Domain Events)
- ✅ Swagger/OpenAPI documentation
- ✅ Docker Compose for local development
