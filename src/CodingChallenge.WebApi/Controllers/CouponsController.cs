using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CodingChallenge.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Coupon>> GetCouponByCouponCode(string couponCode)
    {
        return Ok(new Coupon(Guid.NewGuid(), "Name", "Description", couponCode, 10.1m, 1, 1, new[] { "AA", "BB" }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Coupon>> GetCoupon(Guid id)
    {
        return Ok(new Coupon(id, "Name", "Description", "123", 10.1m, 1, 1, new[] { "AA", "BB" }));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> CreateOrUpdateCoupon(Guid id, [FromBody] Coupon request)
    {
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

        var httpClient = new HttpClient();
        foreach (var coupon in coupons)
        {
            var url = Url.Action(nameof(CreateOrUpdateCoupon), "Coupons", new { id = coupon.Id }, HttpContext.Request.Scheme,
                HttpContext.Request.Host.Value);
            var body = new StringContent(JsonSerializer.Serialize(coupon), Encoding.UTF8,
                MediaTypeNames.Application.Json);

            await httpClient.PutAsync(url, body);
        }
    }

    #endregion
}