namespace CodingChallenge.WebApi.Models;

public record Coupon(Guid Id, string Name, string Description, string CouponCode, double Price, int MaxUsages, int Usages, string[] ProductCodes);