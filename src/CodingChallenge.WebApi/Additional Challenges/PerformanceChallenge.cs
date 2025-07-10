namespace CodingChallenge.WebApi.Additional_Challenges;

public class PerformanceChallenge(IPerformanceChallengeRepository repo)
{
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
        var coupons = repo.GetAll().ToList(); // Materialize once to avoid multiple enumeration
        if (coupons.Count == 0) return 0;
        if (coupons.Count == 1) return 1; // Single coupon always has unique codes

        // Build a frequency map of all product codes (case-insensitive)
        var productCodeFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var coupon in coupons)
        {
            foreach (var productCode in coupon.ProductCodes.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                productCodeFrequency[productCode] = productCodeFrequency.GetValueOrDefault(productCode, 0) + 1;
            }
        }

        // Count coupons that have at least one unique product code
        var result = 0;
        foreach (var coupon in coupons)
        {
            if (coupon.ProductCodes.Any(code => productCodeFrequency[code] == 1))
            {
                result++;
            }
        }

        return result;
    }
}