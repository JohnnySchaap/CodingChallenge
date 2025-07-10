using CodingChallenge.WebApi.CouponModule.Entities;

namespace CodingChallenge.WebApi.CouponModule;

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