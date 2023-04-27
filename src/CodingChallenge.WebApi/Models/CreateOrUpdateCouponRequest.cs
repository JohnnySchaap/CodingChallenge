namespace CodingChallenge.WebApi.Models;

public record CreateOrUpdateCouponRequest(string Name, string Description, string CouponCode, string[] ProductCodes, double Price, int MaxUsages, int CurrentUsages);