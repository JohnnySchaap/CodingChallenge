using CodingChallenge.WebApi.CouponModule;
using CodingChallenge.WebApi.CouponModule.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CodingChallenge.WebApi.Tests.Unit;

// TODO: Cleanup and double check after AI
// TODO: Use Shouldly

public class CouponServiceUnitTests
{
    private readonly ICouponRepository _repository;
    private readonly CouponService _couponService;

    public CouponServiceUnitTests()
    {
        _repository = Substitute.For<ICouponRepository>();
        _couponService = new CouponService(_repository);
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
            Usages: 5,
            ProductCodes: new[] { "PROD1", "PROD2" }
        );

        var existingProducts = new List<ProductEntity> { new() { Id = "prod1" } };

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(existingProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        await _repository.Received(1).GetByIdAsync("test123");
        await _repository.Received(1).GetProductsByCodesAsync(Arg.Is<List<string>>(codes =>
            codes.Count == 2 && codes.Contains("prod1") && codes.Contains("prod2")));
        _repository.Received(1).AddCoupon(Arg.Is<CouponEntity>(entity =>
            entity.Key == coupon.Id &&
            entity.Name == coupon.Name &&
            entity.Description == coupon.Description &&
            entity.Id == "test123" &&
            entity.Price == coupon.Price &&
            entity.MaxUsages == coupon.MaxUsages &&
            entity.Usages == coupon.Usages));
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "prod2"));
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldUpdateExistingCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var coupon = new Coupon(
            Id: couponId,
            Name: "Updated Coupon",
            Description: "Updated Description",
            CouponCode: "EXISTING123",
            Price: 25.00m,
            MaxUsages: 75,
            Usages: 10,
            ProductCodes: new[] { "PROD1", "PROD2" }
        );

        var existingCoupon = new CouponEntity
        {
            Key = couponId,
            Id = "existing123",
            Name = "Old Name",
            Description = "Old Description",
            Price = 15.00m,
            MaxUsages = 50,
            Usages = 5,
            Products = new List<ProductEntity>()
        };

        var existingProducts = new List<ProductEntity> { new() { Id = "prod1" }, new() { Id = "prod2" } };

        _repository.GetByIdAsync("existing123").Returns(existingCoupon);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(existingProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        await _repository.Received(1).GetByIdAsync("existing123");
        await _repository.Received(1).GetProductsByCodesAsync(Arg.Any<List<string>>());
        _repository.DidNotReceive().AddCoupon(Arg.Any<CouponEntity>());
        Assert.Equal("Updated Coupon", existingCoupon.Name);
        Assert.Equal("Updated Description", existingCoupon.Description);
        Assert.Equal(25.00m, existingCoupon.Price);
        Assert.Equal(75, existingCoupon.MaxUsages);
        Assert.Equal(10, existingCoupon.Usages);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldCreateNewProducts_WhenProductsDoNotExist()
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
            ProductCodes: new[] { "NEWPROD1", "NEWPROD2" }
        );

        var emptyProducts = new List<ProductEntity>();

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(emptyProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "newprod1"));
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "newprod2"));
        _repository.Received(1).AddCoupon(Arg.Any<CouponEntity>());
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldReuseExistingProducts_WhenProductsExist()
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
            ProductCodes: new[] { "EXISTING1", "EXISTING2" }
        );

        var existingProducts = new List<ProductEntity> { new() { Id = "existing1" }, new() { Id = "existing2" } };

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(existingProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        _repository.DidNotReceive().AddProduct(Arg.Any<ProductEntity>());
        _repository.Received(1).AddCoupon(Arg.Any<CouponEntity>());
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleMixedExistingAndNewProducts()
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
            ProductCodes: new[] { "EXISTING1", "NEWPROD1" }
        );

        var existingProducts = new List<ProductEntity> { new() { Id = "existing1" } };

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(existingProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "newprod1"));
        _repository.DidNotReceive().AddProduct(Arg.Is<ProductEntity>(p => p.Id == "existing1"));
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleEmptyProductCodes()
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
            ProductCodes: Array.Empty<string>()
        );

        var emptyProducts = new List<ProductEntity>();

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(emptyProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        await _repository.Received(1).GetProductsByCodesAsync(Arg.Is<List<string>>(codes => codes.Count == 0));
        _repository.DidNotReceive().AddProduct(Arg.Any<ProductEntity>());
        _repository.Received(1).AddCoupon(Arg.Any<CouponEntity>());
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldConvertCouponCodeToLowerCase()
    {
        // Arrange
        var coupon = new Coupon(
            Id: Guid.NewGuid(),
            Name: "Test Coupon",
            Description: "Test Description",
            CouponCode: "UPPERCASE123",
            Price: 10.50m,
            MaxUsages: 100,
            Usages: 0,
            ProductCodes: new[] { "PROD1" }
        );

        var existingProducts = new List<ProductEntity> { new() { Id = "prod1" } };

        _repository.GetByIdAsync("uppercase123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(existingProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        await _repository.Received(1).GetByIdAsync("uppercase123");
        _repository.Received(1).AddCoupon(Arg.Is<CouponEntity>(entity => entity.Id == "uppercase123"));
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldConvertProductCodesToLowerCase()
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
            ProductCodes: new[] { "UPPERCASE", "MixedCase" }
        );

        var emptyProducts = new List<ProductEntity>();

        _repository.GetByIdAsync("test123").Returns((CouponEntity?)null);
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(emptyProducts);

        // Act
        await _couponService.CreateOrUpdateCoupon(coupon);

        // Assert
        await _repository.Received(1).GetProductsByCodesAsync(Arg.Is<List<string>>(codes =>
            codes.Contains("uppercase") && codes.Contains("mixedcase")));
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "uppercase"));
        _repository.Received(1).AddProduct(Arg.Is<ProductEntity>(p => p.Id == "mixedcase"));
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldReturnCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var couponEntity = new CouponEntity
        {
            Key = couponId,
            Id = "test123",
            Name = "Test Coupon",
            Description = "Test Description",
            Price = 10.50m,
            MaxUsages = 100,
            Usages = 5,
            Products = new List<ProductEntity> { new() { Id = "prod1" }, new() { Id = "prod2" } }
        };

        _repository.GetWithProductsAsync("test123").Returns(couponEntity);

        // Act
        var result = await _couponService.GetCoupon("TEST123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(couponId, result.Id);
        Assert.Equal("Test Coupon", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("test123", result.CouponCode);
        Assert.Equal(10.50m, result.Price);
        Assert.Equal(100, result.MaxUsages);
        Assert.Equal(5, result.Usages);
        Assert.Equal(2, result.ProductCodes.Length);
        Assert.Contains("prod1", result.ProductCodes);
        Assert.Contains("prod2", result.ProductCodes);
        await _repository.Received(1).GetWithProductsAsync("test123");
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldReturnNull_WhenCouponDoesNotExist()
    {
        // Arrange
        _repository.GetWithProductsAsync("nonexistent").Returns((CouponEntity?)null);

        // Act
        var result = await _couponService.GetCoupon("NONEXISTENT");

        // Assert
        Assert.Null(result);
        await _repository.Received(1).GetWithProductsAsync("nonexistent");
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldConvertCouponCodeToLowerCase()
    {
        // Arrange
        _repository.GetWithProductsAsync("uppercase123").Returns((CouponEntity?)null);

        // Act
        await _couponService.GetCoupon("UPPERCASE123");

        // Assert
        await _repository.Received(1).GetWithProductsAsync("uppercase123");
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldHandleEmptyProductList()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var couponEntity = new CouponEntity
        {
            Key = couponId,
            Id = "test123",
            Name = "Test Coupon",
            Description = "Test Description",
            Price = 10.50m,
            MaxUsages = 100,
            Usages = 5,
            Products = new List<ProductEntity>()
        };

        _repository.GetWithProductsAsync("test123").Returns(couponEntity);

        // Act
        var result = await _couponService.GetCoupon("TEST123");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ProductCodes);
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldReturnCoupon_WhenCouponExists()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var couponEntity = new CouponEntity
        {
            Key = couponId,
            Id = "test123",
            Name = "Test Coupon",
            Description = "Test Description",
            Price = 10.50m,
            MaxUsages = 100,
            Usages = 5,
            Products = new List<ProductEntity> { new() { Id = "prod1" }, new() { Id = "prod2" } }
        };

        _repository.GetWithProductsByKeyAsync(couponId).Returns(couponEntity);

        // Act
        var result = await _couponService.GetCoupon(couponId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(couponId, result.Id);
        Assert.Equal("Test Coupon", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("test123", result.CouponCode);
        Assert.Equal(10.50m, result.Price);
        Assert.Equal(100, result.MaxUsages);
        Assert.Equal(5, result.Usages);
        Assert.Equal(2, result.ProductCodes.Length);
        Assert.Contains("prod1", result.ProductCodes);
        Assert.Contains("prod2", result.ProductCodes);
        await _repository.Received(1).GetWithProductsByKeyAsync(couponId);
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldReturnNull_WhenCouponDoesNotExist()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        _repository.GetWithProductsByKeyAsync(couponId).Returns((CouponEntity?)null);

        // Act
        var result = await _couponService.GetCoupon(couponId);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).GetWithProductsByKeyAsync(couponId);
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldHandleEmptyProductList()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        var couponEntity = new CouponEntity
        {
            Key = couponId,
            Id = "test123",
            Name = "Test Coupon",
            Description = "Test Description",
            Price = 10.50m,
            MaxUsages = 100,
            Usages = 5,
            Products = new List<ProductEntity>()
        };

        _repository.GetWithProductsByKeyAsync(couponId).Returns(couponEntity);

        // Act
        var result = await _couponService.GetCoupon(couponId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ProductCodes);
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleNullProductsCollection()
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
            ProductCodes: null!
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _couponService.CreateOrUpdateCoupon(coupon));
    }

    [Fact]
    public async Task CreateOrUpdateCoupon_ShouldHandleRepositoryException()
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
            ProductCodes: []
        );
        _repository.GetProductsByCodesAsync(Arg.Any<List<string>>()).Returns(new List<ProductEntity>());

        _repository.GetByIdAsync("test123").Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _couponService.CreateOrUpdateCoupon(coupon));
    }

    [Fact]
    public async Task GetCoupon_ByCouponCode_ShouldHandleRepositoryException()
    {
        // Arrange
        _repository.GetWithProductsAsync("test123").Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _couponService.GetCoupon("TEST123"));
    }

    [Fact]
    public async Task GetCoupon_ById_ShouldHandleRepositoryException()
    {
        // Arrange
        var couponId = Guid.NewGuid();
        _repository.GetWithProductsByKeyAsync(couponId).Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _couponService.GetCoupon(couponId));
    }
}