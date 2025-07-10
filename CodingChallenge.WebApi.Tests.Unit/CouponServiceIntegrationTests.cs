using CodingChallenge.WebApi.CouponModule;
using CodingChallenge.WebApi.CouponModule.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.WebApi.Tests.Unit;

// TODO: Cleanup and double check after AI
// TODO: Use Shouldly
// TODO: Move to integration test lib

public class CouponServiceIntegrationTests : IDisposable
{
    private readonly CouponDbContext _context;
    private readonly CouponService _couponService;

    public CouponServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<CouponDbContext>()
            .UseSqlite($"Data Source={Guid.NewGuid()}.db")
            .Options;
        _context = new CouponDbContext(options);
        _context.Database.EnsureCreated();
        _couponService = new CouponService(new CouponRepository(_context));
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldCreateNewCoupon_WhenCouponDoesNotExist()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "TEST123",
            Price: 10.50m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "PROD1", "PROD2" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var savedCoupon = await _context.Coupons
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == "test123");

        Assert.NotNull(savedCoupon);
        Assert.Equal(coupon.Id, savedCoupon.Key);
        Assert.Equal(coupon.Name, savedCoupon.Name);
        Assert.Equal(coupon.Description, savedCoupon.Description);
        Assert.Equal("test123", savedCoupon.Id);
        Assert.Equal(coupon.Price, savedCoupon.Price);
        Assert.Equal(coupon.MaxUsages, savedCoupon.MaxUsages);
        Assert.Equal(coupon.Usages, savedCoupon.Usages);
        Assert.Equal(2, savedCoupon.Products.Count);
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod1");
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod2");
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldUpdateExistingCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var originalCoupon = new Coupon(
            Id: couponId,
            Name: "Original Name",
            Description: "Original Description",
            CouponCode: "EXISTING123",
            Price: 15.00m,
            MaxUsages: 50,
            Usages: 5,
            ProductCodes: new[] { "PROD1" }
        );

        await _couponService.CreateOrUpdateCoupon(originalCoupon);

        var updatedCoupon = new Coupon(
            Id: couponId,
            Name: "Updated Name",
            Description: "Updated Description",
            CouponCode: "EXISTING123",
            Price: 25.00m,
            MaxUsages: 75,
            Usages: 10,
            ProductCodes: new[] { "PROD2", "PROD3" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(updatedCoupon);

        // Assert
        var savedCoupon = await _context.Coupons
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == "existing123");

        Assert.NotNull(savedCoupon);
        Assert.Equal(updatedCoupon.Id, savedCoupon.Key);
        Assert.Equal("Updated Name", savedCoupon.Name);
        Assert.Equal("Updated Description", savedCoupon.Description);
        Assert.Equal(25.00m, savedCoupon.Price);
        Assert.Equal(75, savedCoupon.MaxUsages);
        Assert.Equal(10, savedCoupon.Usages);
        Assert.Equal(2, savedCoupon.Products.Count);
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod2");
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod3");
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldCreateNewProducts_WhenProductsDoNotExist()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "NEW123",
            Price: 10.50m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "NEWPROD1", "NEWPROD2" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var products = await _context.Products.ToListAsync();
        Assert.Contains(products, p => p.Id == "newprod1");
        Assert.Contains(products, p => p.Id == "newprod2");
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldReuseExistingProducts_WhenProductsExist()
    {
        // Arrange
        var existingProduct = new ProductEntity { Id = "existing-prod" };
        _context.Products.Add(existingProduct);
        await _context.SaveChangesAsync();

        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "REUSE123",
            Price: 10.50m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "EXISTING-PROD", "NEW-PROD" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var products = await _context.Products.ToListAsync();
        Assert.Equal(2, products.Count);
        Assert.Contains(products, p => p.Id == "existing-prod");
        Assert.Contains(products, p => p.Id == "new-prod");
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldReturnCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var coupon = new Coupon(
            Id: couponId,
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "GETTEST123",
            Price: 20.00m,
            MaxUsages: 200,
            Usages: 15,
            ProductCodes: new[] { "PROD1", "PROD2" }
        );

        await _couponService.CreateOrUpdateCoupon(coupon);

        // Act
        var result = await _couponService.GetCoupon("GETTEST123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(couponId, result.Id);
        Assert.Equal("Test Coupon", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("gettest123", result.CouponCode);
        Assert.Equal(20.00m, result.Price);
        Assert.Equal(200, result.MaxUsages);
        Assert.Equal(15, result.Usages);
        Assert.Equal(2, result.ProductCodes.Length);
        Assert.Contains("prod1", result.ProductCodes);
        Assert.Contains("prod2", result.ProductCodes);
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldReturnNull_WhenCouponDoesNotExist()
    {
        // Act
        var result = await _couponService.GetCoupon("NONEXISTENT");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldReturnCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var coupon = new Coupon(
            Id: couponId,
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "GETBYID123",
            Price: 30.00m,
            MaxUsages: 300,
            Usages: 25,
            ProductCodes: new[] { "PROD3", "PROD4" }
        );

        await _couponService.CreateOrUpdateCoupon(coupon);

        // Act
        var result = await _couponService.GetCoupon(couponId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(couponId, result.Id);
        Assert.Equal("Test Coupon", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("getbyid123", result.CouponCode);
        Assert.Equal(30.00m, result.Price);
        Assert.Equal(300, result.MaxUsages);
        Assert.Equal(25, result.Usages);
        Assert.Equal(2, result.ProductCodes.Length);
        Assert.Contains("prod3", result.ProductCodes);
        Assert.Contains("prod4", result.ProductCodes);
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldReturnNull_WhenCouponDoesNotExist()
    {
        // Act
        var result = await _couponService.GetCoupon(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleEmptyProductCodes()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "No Products Coupon",
            Description: "Test Description",
            CouponCode: "NOPRODUCTS123",
            Price: 5.00m,
            MaxUsages: 50,
            Usages: 0,
            ProductCodes: Array.Empty<string>()
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var savedCoupon = await _context.Coupons
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == "noproducts123");

        Assert.NotNull(savedCoupon);
        Assert.Empty(savedCoupon.Products);
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleCaseInsensitiveProductCodes()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Case Test Coupon",
            Description: "Test Description",
            CouponCode: "CASETEST123",
            Price: 12.00m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "MixedCase", "UPPERCASE", "lowercase" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var savedCoupon = await _context.Coupons
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == "casetest123");

        Assert.NotNull(savedCoupon);
        Assert.Equal(3, savedCoupon.Products.Count);
        Assert.Contains(savedCoupon.Products, p => p.Id == "mixedcase");
        Assert.Contains(savedCoupon.Products, p => p.Id == "uppercase");
        Assert.Contains(savedCoupon.Products, p => p.Id == "lowercase");
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleDuplicateProductCodes()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Duplicate Test Coupon",
            Description: "Test Description",
            CouponCode: "DUPTEST123",
            Price: 8.00m,
            MaxUsages: 80,
            Usages: 0,
            ProductCodes: new[] { "PROD1", "prod1", "PROD2" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        var savedCoupon = await _context.Coupons
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == "duptest123");

        Assert.NotNull(savedCoupon);
        Assert.Equal(2, savedCoupon.Products.Count);
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod1");
        Assert.Contains(savedCoupon.Products, p => p.Id == "prod2");
    }

    [Fact]
    public async Task Multiple_Operations_ShouldWork_InSequence()
    {
        // Arrange
        var coupon1 = new Coupon(
            Id: Guid.NewGuid(),
            Name: "First Coupon",
            Description: "First Description",
            CouponCode: "FIRST123",
            Price: 10.00m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "SHARED", "UNIQUE1" }
        );

        var coupon2 = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Second Coupon",
            Description: "Second Description",
            CouponCode: "SECOND123",
            Price: 20.00m,
            MaxUsages: 200,
            Usages: 5,
            ProductCodes: new[] { "SHARED", "UNIQUE2" }
        );

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon1);
        await _couponService.CreateOrUpdateCoupon(coupon2);

        var retrieved1 = await _couponService.GetCoupon("FIRST123");
        var retrieved2 = await _couponService.GetCoupon(coupon2.Id);

        // Assert
        Assert.NotNull(retrieved1);
        Assert.NotNull(retrieved2);
        Assert.Equal("First Coupon", retrieved1.Name);
        Assert.Equal("Second Coupon", retrieved2.Name);

        var products = await _context.Products.ToListAsync();
        Assert.Equal(3, products.Count); // shared, unique1, unique2
        Assert.Contains(products, p => p.Id == "shared");
        Assert.Contains(products, p => p.Id == "unique1");
        Assert.Contains(products, p => p.Id == "unique2");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}