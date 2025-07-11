using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CodingChallenge.WebApi.AuthenticationModule;
using CodingChallenge.WebApi.CouponModule;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.WebApi.Controllers;

// TODO: Add Swagger documentation via XML doc so downstream service devs can integrate easier 

[ApiController]
[Route("[controller]")]
public class CouponsController(ICouponService couponService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Coupon>> GetCouponByCouponCode(string couponCode)
    {
        // TODO: Validate input, throw corresponding errors
        return Ok(await couponService.GetCoupon(couponCode));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Coupon>> GetCoupon(Guid id)
    {
        return Ok(await couponService.GetCoupon(id));
    }

    [HttpGet("{couponCode}/availability")]
    public async Task<ActionResult<Coupon>> CanUseCoupon(string couponCode)
    {
        // TODO: Add product validation by the couponCode
        return Ok(await couponService.CanUseCoupon(couponCode));
    }

    [HttpPost("{couponCode}/usage")]
    public async Task<ActionResult<Coupon>> ApplyCoupon(string couponCode)
    {
        // TODO: Add product validation by the couponCode
        return Ok(await couponService.ApplyCoupon(couponCode));
    }

    [HttpDelete("{couponCode}/usage")]
    public Task<ActionResult<Coupon>> DecrementCoupon(string couponCode)
    {
        // TODO: Implement
        // TODO: Consider to split to 2 methods to distinguish multiple use cases: rollback, marketing adjustment
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> CreateOrUpdateCoupon(Guid id, [FromBody] Coupon request)
    {
        // TODO: Validate input, throw corresponding errors
        // TODO: Consider to introduce PATCH method for partial updates, e.g. Description only
        await couponService.CreateOrUpdateCoupon(request with { Id = id });
        return Ok();
    }

    #region All code below is not going to be reviewed. This is just some helpers to load the data.

    private const string DataFileName = @"data.json";

    /// <summary>
    /// This endpoint is created so that you can easily populate your data.
    /// By doing it like this, we simulating the user to call the CreateOrUpdateCoupon endpoint.
    /// You can do it any other way you like. We did it like this so that you are not forced in any direction on how you will store your data.
    /// If you leave it like this, make sure you run this endpoint every time you start up though.
    /// </summary>
    [HttpGet("Test/[action]")]
    public async Task Populate()
    {
        var data = await System.IO.File.ReadAllTextAsync(DataFileName);
        var coupons = JsonSerializer.Deserialize<List<Coupon>>(data);

        var httpClient = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {
                    AuthenticationConstants.ApiKeyHeaderName,
                    Request.Headers[AuthenticationConstants.ApiKeyHeaderName].ToString()
                }
            }
        };
        foreach (var coupon in coupons)
        {
            var url = Url.Action(nameof(CreateOrUpdateCoupon), "Coupons", new { id = coupon.Id },
                HttpContext.Request.Scheme,
                HttpContext.Request.Host.Value);
            var body = new StringContent(JsonSerializer.Serialize(coupon), Encoding.UTF8,
                MediaTypeNames.Application.Json);

            var response = await httpClient.PutAsync(url, body);
            response.EnsureSuccessStatusCode();
        }
    }

    #endregion
}