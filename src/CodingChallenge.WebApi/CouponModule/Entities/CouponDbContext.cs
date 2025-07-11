using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.WebApi.CouponModule.Entities;

public class CouponDbContext : DbContext
{
    public CouponDbContext()
    {
    }

    public CouponDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CouponEntity>(t =>
        {
            t.HasMany(e => e.Products).WithMany(e => e.Coupons).UsingEntity("CouponProduct");
        });
    }
}