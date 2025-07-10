namespace CodingChallenge.WebApi.CouponModule.Entities;

public interface ICouponRepository
{
    Task<CouponEntity?> GetByIdAsync(string couponCode);
    Task<CouponEntity?> GetByKeyAsync(Guid key);
    Task<CouponEntity?> GetWithProductsAsync(string couponCode);
    Task<CouponEntity?> GetWithProductsByKeyAsync(Guid key);
    Task<List<ProductEntity>> GetProductsByCodesAsync(List<string> productCodes);
    void AddCoupon(CouponEntity coupon);
    void AddProduct(ProductEntity product);
    Task SaveChangesAsync();
}