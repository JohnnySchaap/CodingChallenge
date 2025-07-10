using CodingChallenge.WebApi.CouponModule;
using CodingChallenge.WebApi.Models;

namespace CodingChallenge.WebApi.Additional_Challenges;

public interface IPerformanceChallengeRepository
{
    IEnumerable<Coupon> GetAll();
}
