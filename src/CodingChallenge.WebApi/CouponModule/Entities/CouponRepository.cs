using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.WebApi.CouponModule.Entities;

public class CouponRepository(CouponDbContext context) : ICouponRepository
{
    public async Task<CouponEntity?> GetByIdAsync(string couponCode)
    {
        return await context.Coupons.FindAsync(couponCode.ToLower());
    }

    public async Task<CouponEntity?> GetByKeyAsync(Guid key)
    {
        return await context.Coupons.FirstOrDefaultAsync(c => c.Key == key);
    }

    public async Task<CouponEntity?> GetWithProductsAsync(string couponCode)
    {
        return await context.Coupons
            .Include(e => e.Products)
            .FirstOrDefaultAsync(e => e.Id == couponCode.ToLower());
    }

    public async Task<CouponEntity?> GetWithProductsByKeyAsync(Guid key)
    {
        return await context.Coupons
            .Include(e => e.Products)
            .FirstOrDefaultAsync(e => e.Key == key);
    }

    public async Task<List<ProductEntity>> GetProductsByCodesAsync(List<string> productCodes)
    {
        return await context.Products
            .Where(e => productCodes.Contains(e.Id))
            .ToListAsync();
    }

    public void AddCoupon(CouponEntity coupon)
    {
        context.Coupons.Add(coupon);
    }

    public void AddProduct(ProductEntity product)
    {
        context.Products.Add(product);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}