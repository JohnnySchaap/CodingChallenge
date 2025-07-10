namespace CodingChallenge.WebApi.CouponModule.Entities;

public class ProductEntity
{
    public string Id { get; set; }
    public List<CouponEntity> Coupons { get; set; }
}