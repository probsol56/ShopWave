using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWave.Domain.Entities;

namespace ShopWave.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Currency).IsRequired().HasMaxLength(3);
        builder.HasMany(s => s.Categories).WithOne(c => c.Shop).HasForeignKey(c => c.ShopId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Products).WithOne(p => p.Shop).HasForeignKey(p => p.ShopId).OnDelete(DeleteBehavior.Cascade);
    }
}
