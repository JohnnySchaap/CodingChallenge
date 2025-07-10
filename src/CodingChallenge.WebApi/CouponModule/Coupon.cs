namespace CodingChallenge.WebApi.CouponModule;

public record Coupon(Guid Id, string Name, string Description, string CouponCode, decimal Price, int MaxUsages, int Usages, string[] ProductCodes);