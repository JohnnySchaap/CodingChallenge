using CodingChallenge.WebApi.CouponModule.Entities;

namespace CodingChallenge.WebApi.CouponModule;

// TODO: Extract mapper to own class, cover by tests
// TODO: Add logs to be able to investigate PROD issues

public class CouponService(ICouponRepository repository) : ICouponService
{
    public async Task CreateOrUpdateCoupon(Coupon coupon)
    {
        var productCodes = coupon.ProductCodes.Select(e => e.ToLower()).ToList();
        var products = await CreateOrUpdateProducts(productCodes);
        var existing = await repository.GetByIdAsync(coupon.CouponCode.ToLower());

        if (existing == null)
        {
            var entity = new CouponEntity();
            MapDtoToEntity(coupon, entity);
            entity.Products = products;
            repository.AddCoupon(entity);
        }
        else
        {
            MapDtoToEntity(coupon, existing);
            existing.Products = products;
        }

        await repository.SaveChangesAsync();
    }

    public async Task<Coupon?> GetCoupon(string couponCode)
    {
        var entity = await repository.GetWithProductsAsync(couponCode.ToLower());
        return entity == null ? null : ConvertEntityToDto(entity);
    }

    public async Task<Coupon?> GetCoupon(Guid id)
    {
        var entity = await repository.GetWithProductsByKeyAsync(id);
        return entity == null ? null : ConvertEntityToDto(entity);
    }

    private async Task<List<ProductEntity>> CreateOrUpdateProducts(List<string> productCodes)
    {
        var products = await repository.GetProductsByCodesAsync(productCodes);
        var existingProductCodes = products.Select(p => p.Id).ToList();
        var productCodesToCreate = productCodes.Except(existingProductCodes).ToList();

        foreach (var productCode in productCodesToCreate)
        {
            var product = new ProductEntity { Id = productCode };
            repository.AddProduct(product);
            products.Add(product);
        }

        return products;
    }

    public async Task<bool> CanUseCoupon(string couponCode)
    {
        var coupon = await repository.GetByIdAsync(couponCode);
        return coupon != null && CanUseCoupon(coupon);
    }

    public async Task<bool> ApplyCoupon(string couponCode)
    {
        var coupon = await repository.GetByIdAsync(couponCode);
        if (coupon == null || !CanUseCoupon(coupon))
            return false;

        coupon.Usages++;
        var updated = await repository.SaveChangesAsync();
        var isApplied = updated > 0;
        return isApplied;
    }

    private static bool CanUseCoupon(CouponEntity coupon)
    {
        // MaxUsages = 0 means unlimited usage
        if (coupon.MaxUsages == 0)
        {
            return true; // Can always be used
        }

        // For limited usage coupons, check if usage count is less than max
        return coupon.Usages < coupon.MaxUsages;
    }

    private static void MapDtoToEntity(Coupon dto, CouponEntity entity)
    {
        entity.Key = dto.Id;
        entity.Name = dto.Name;
        entity.Id = dto.CouponCode.ToLower();
        entity.Description = dto.Description;
        entity.MaxUsages = dto.MaxUsages;
        entity.Price = dto.Price;
        entity.Usages = dto.Usages;
    }

    private static Coupon ConvertEntityToDto(CouponEntity entity)
    {
        return new Coupon(
            Id: entity.Key,
            Name: entity.Name,
            Description: entity.Description,
            CouponCode: entity.Id,
            Price: entity.Price,
            MaxUsages: entity.MaxUsages,
            Usages: entity.Usages,
            ProductCodes: entity.Products.Select(e => e.Id).ToArray());
    }
}