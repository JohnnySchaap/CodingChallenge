namespace CodingChallenge.WebApi.Models;

public record GetCouponResponse(Guid Id, string Name, string Description, string CouponCode, string[] ProductCodes, double Price, int MaxUsages, int CurrentUsages);