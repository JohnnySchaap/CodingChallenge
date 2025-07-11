namespace CodingChallenge.WebApi.CouponModule;

public interface ICouponService
{
    Task CreateOrUpdateCoupon(Coupon coupon);
    Task<Coupon?> GetCoupon(Guid id);
    Task<Coupon?> GetCoupon(string couponCode);
    Task<bool> CanUseCoupon(string couponCode);
    Task<bool> ApplyCoupon(string couponCode);
}