using CodingChallenge.WebApi.Models;

namespace CodingChallenge.WebApi.Additional_Challenges;

public class PerformanceChallenge
{
    private readonly IPerformanceChallengeRepository _repo;

    public PerformanceChallenge(IPerformanceChallengeRepository repo)
    {
        _repo = repo;
    }

    // TODO-001: Challenge: This code is really slow. Can you refactor it in such a way that it is much faster?
    // TODO-002: Challenge: Can you write unit tests that test all possible scenarios.

    /// <summary>
    /// Counts coupons that have at least one unique product code (not used by other coupons)
    /// Example: 
    /// Coupon 1: [ PC1, PC2, PC3]
    /// Coupon 2: [ PC2, PC3 ] 
    /// As Coupon 1 is the only coupon that contains PC1, the count is 1.
    /// </summary>
    /// <param name="coupons">Coupons to check</param>
    /// <remarks>All coupons have at least 1 product code</remarks>
    /// <remarks>A coupon contains distinct product codes</remarks>
    /// <remarks>Product codes comparison is case insensitive</remarks>
    /// <returns>Returns the number of coupons that have at least one unique product code</returns>
    public int CountCouponsWithUniqueProductCodes()
    {
        var coupons = _repo.GetAll();

        var result = 0;

        foreach (var coupon1 in coupons)
        {
            var productCodes = coupon1.ProductCodes.ToList();

            foreach (var coupon2 in coupons.Where(x => x != coupon1))
            {
                productCodes.RemoveAll(x => coupon2.ProductCodes.Contains(x, StringComparer.OrdinalIgnoreCase));
            }

            if (productCodes.Any())
            {
                result++;
            }
        }

        return result;
    }
}