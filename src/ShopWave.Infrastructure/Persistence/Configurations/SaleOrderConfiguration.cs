using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWave.Domain.Entities;

namespace ShopWave.Infrastructure.Persistence.Configurations;

public class SaleOrderConfiguration : IEntityTypeConfiguration<SaleOrder>
{
    public void Configure(EntityTypeBuilder<SaleOrder> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 4);
        builder.HasMany(o => o.Items).WithOne(i => i.SaleOrder).HasForeignKey(i => i.SaleOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SaleOrderItemConfiguration : IEntityTypeConfiguration<SaleOrderItem>
{
    public void Configure(EntityTypeBuilder<SaleOrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 4);
        builder.HasOne(i => i.Product).WithMany(p => p.SaleOrderItems).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
