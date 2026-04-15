using Microsoft.EntityFrameworkCore;
using ShopWave.Domain.Entities;

namespace ShopWave.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Shop> Shops { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<StockEntry> StockEntries { get; }
    DbSet<SaleOrder> SaleOrders { get; }
    DbSet<SaleOrderItem> SaleOrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
