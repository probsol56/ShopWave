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

        // Global query filters for multi-tenancy
        // When no tenant is resolved (e.g. public endpoints like /register), the filter
        // is skipped so the query runs without a tenant constraint.
        modelBuilder.Entity<User>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Shop>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<StockEntry>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<SaleOrder>().HasQueryFilter(e => !tenantContext.IsResolved || e.TenantId == tenantContext.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
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
