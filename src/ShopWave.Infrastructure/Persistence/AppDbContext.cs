using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Domain.Common;
using ShopWave.Domain.Entities;

namespace ShopWave.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext, ICurrentUserService currentUserService)
    : DbContext(options), IAppDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockEntry> StockEntries => Set<StockEntry>();
    public DbSet<SaleOrder> SaleOrders => Set<SaleOrder>();
    public DbSet<SaleOrderItem> SaleOrderItems => Set<SaleOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for multi-tenancy.
        // ResolvedTenantId returns Guid.Empty (never throws) when no tenant is set (e.g. public
        // endpoints like /register). In that case the filter is a no-op and all rows are visible.
       ApplyTenantFilters(modelBuilder);
    }

    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        var tenantEntityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(ITenantEntity).IsAssignableFrom(t.ClrType));

        foreach (var entityType in tenantEntityTypes)
        {
            var param = Expression.Parameter(entityType.ClrType, "e");
            var tenantIdProp = Expression.Property(param, nameof(ITenantEntity.TenantId));

            // e.TenantId == tenantContext.ResolvedTenantId
            var resolvedId = Expression.Constant(tenantContext.ResolvedTenantId);
            var tenantMatch = Expression.Equal(tenantIdProp, resolvedId);

            // tenantContext.ResolvedTenantId == Guid.Empty || ...
            var isEmpty = Expression.Equal(resolvedId, Expression.Constant(Guid.Empty));
            var filter = Expression.OrElse(isEmpty, tenantMatch);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(Expression.Lambda(filter, param));
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserService.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = currentUserService.UserId;
            }
        }

        // Auto-set TenantId on new entities (only when a tenant is resolved)
        if (tenantContext.IsResolved)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                    entry.Entity.TenantId = tenantContext.TenantId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
