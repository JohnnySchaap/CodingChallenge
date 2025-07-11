namespace CodingChallenge.WebApi.CouponModule.Entities;

// TODO: Implement optimistic blocking via RowVersion for concurrency cases

public class CouponEntity
{
    public string Id { get; set; }
    public Guid Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int MaxUsages { get; set; }
    public int Usages { get; set; }
    public List<ProductEntity> Products { get; set; }
    // [Timestamp] public byte[] RowVersion { get; set; }
}