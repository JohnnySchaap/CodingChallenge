using CodingChallenge.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponController : ControllerBase
{
    private readonly ILogger<CouponController> _logger;

    public CouponController(ILogger<CouponController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<GetCouponResponse> GetCoupon(Guid id)
    {
        return Ok(new GetCouponResponse(id, "Name", "Description", "123", new[] { "AA", "BB" }, 10.1, 1, 1));
    }

    [HttpPut]
    [Route("{id}")]
    public ActionResult CreateOrUpdateCoupon(Guid id, [FromBody] CreateOrUpdateCouponRequest request)
    {
        return Ok();
    }
}