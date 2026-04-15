using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Domain.Common;
using ShopWave.Domain.Entities;

namespace ShopWave.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
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
        modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Shop>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Category>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<Product>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<StockEntry>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        modelBuilder.Entity<SaleOrder>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        // Auto-set TenantId on new entities
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                entry.Entity.TenantId = tenantContext.TenantId;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
