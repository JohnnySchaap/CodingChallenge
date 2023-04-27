using System.Reflection;
using CodingChallenge.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.WebApi.Controllers;

[Route("")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppInformationResponse))]
    public ActionResult<AppInformationResponse> Get()
    {
        var result = new AppInformationResponse(
            "Coding Challenge",
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            Environment.MachineName
        );

        return Ok(result);
    }
}