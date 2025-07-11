using CodingChallenge.WebApi.Additional_Challenges;
using CodingChallenge.WebApi.CouponModule;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CodingChallenge.WebApi.Tests.Unit;

public class PerformanceChallengeTests
{
    private readonly IPerformanceChallengeRepository _repository;
    private readonly PerformanceChallenge _performanceChallenge;

    public PerformanceChallengeTests()
    {
        _repository = Substitute.For<IPerformanceChallengeRepository>();
        _performanceChallenge = new PerformanceChallenge(_repository);
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnZero_WhenNoCouponsExist()
    {
        // Arrange
        _repository.GetAll().Returns(new List<Coupon>());

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnOne_WhenOneCouponExists()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Single Coupon",
                Description: "Description",
                CouponCode: "SINGLE",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "PROD1", "PROD2" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnOne_WhenTwoCouponsHaveOneUniqueProductCode()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "UNIQUE1", "SHARED" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "SHARED" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result); // Only the first coupon has unique product code "UNIQUE1"
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnTwo_WhenTwoCouponsHaveUniqueProductCodes()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "UNIQUE1", "SHARED" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "UNIQUE2", "SHARED" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(2, result); // Both coupons have unique product codes
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnZero_WhenAllProductCodesAreShared()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "SHARED1", "SHARED2" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "SHARED1", "SHARED2" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(0, result); // No coupon has unique product codes
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleCaseInsensitiveComparison()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "UNIQUE1", "shared" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "SHARED" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result); // Only the first coupon has unique product code "UNIQUE1"
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleComplexScenario()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "PC1", "PC2", "PC3" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "PC2", "PC3" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 3",
                Description: "Description",
                CouponCode: "COUP3",
                Price: 20.0m,
                MaxUsages: 25,
                Usages: 0,
                ProductCodes: new[] { "PC4", "PC5" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 4",
                Description: "Description",
                CouponCode: "COUP4",
                Price: 25.0m,
                MaxUsages: 10,
                Usages: 0,
                ProductCodes: new[] { "PC5", "PC6" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(3, result); // Coupon 1 (PC1), Coupon 3 (PC4), Coupon 4 (PC6)
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleEmptyProductCodes()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Empty Coupon",
                Description: "Description",
                CouponCode: "EMPTY",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: Array.Empty<string>()
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Normal Coupon",
                Description: "Description",
                CouponCode: "NORMAL",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "PROD1" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result); // Only the normal coupon has unique product codes
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleMultipleEmptyProductCodes()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Empty Coupon 1",
                Description: "Description",
                CouponCode: "EMPTY1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: Array.Empty<string>()
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Empty Coupon 2",
                Description: "Description",
                CouponCode: "EMPTY2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: Array.Empty<string>()
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(0, result); // No coupon has unique product codes
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleDuplicateProductCodesWithinSameCoupon()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon with Duplicates",
                Description: "Description",
                CouponCode: "DUPE",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "PROD1", "PROD1", "PROD2" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Other Coupon",
                Description: "Description",
                CouponCode: "OTHER",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "PROD2" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result); // First coupon has unique "PROD1" (duplicates are handled by ToList())
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleMixedCaseProductCodes()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "prod1", "PROD2" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "PROD1", "prod3" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(2, result); // Both coupons have unique product codes (case insensitive comparison)
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleRepositoryException()
    {
        // Arrange
        _repository.GetAll().Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _performanceChallenge.CountCouponsWithUniqueProductCodes());
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleNullFromRepository()
    {
        // Arrange
        _repository.GetAll().Returns((IEnumerable<Coupon>)null!);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _performanceChallenge.CountCouponsWithUniqueProductCodes());
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldHandleLargeCouponSet()
    {
        // Arrange
        var coupons = new List<Coupon>();
        for (int i = 0; i < 100; i++)
        {
            coupons.Add(new Coupon(
                Id: Guid.NewGuid(),
                Name: $"Coupon {i}",
                Description: "Description",
                CouponCode: $"COUP{i}",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { $"UNIQUE{i}", "SHARED" }
            ));
        }

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(100, result); // Each coupon has a unique product code
    }

    [Fact]
    public void CountCouponsWithUniqueProductCodes_ShouldReturnCorrectCount_WhenSubsetOfCouponsHaveUniqueProductCodes()
    {
        // Arrange - Example from the method documentation
        var coupons = new List<Coupon>
        {
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 1",
                Description: "Description",
                CouponCode: "COUP1",
                Price: 10.0m,
                MaxUsages: 100,
                Usages: 0,
                ProductCodes: new[] { "PC1", "PC2", "PC3" }
            ),
            new Coupon(
                Id: Guid.NewGuid(),
                Name: "Coupon 2",
                Description: "Description",
                CouponCode: "COUP2",
                Price: 15.0m,
                MaxUsages: 50,
                Usages: 0,
                ProductCodes: new[] { "PC2", "PC3" }
            )
        };

        _repository.GetAll().Returns(coupons);

        // Act
        var result = _performanceChallenge.CountCouponsWithUniqueProductCodes();

        // Assert
        Assert.Equal(1, result); // Only Coupon 1 has unique product code "PC1"
    }
}