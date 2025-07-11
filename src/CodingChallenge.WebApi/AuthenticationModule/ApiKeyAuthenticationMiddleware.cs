using Microsoft.Extensions.Options;

namespace CodingChallenge.WebApi.AuthenticationModule;

public class ApiKeyAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<AuthenticationConfig> config)
{
    // TODO: Add logger
    // TODO: Cover by tests

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for Swagger endpoints
        var request = context.Request;
        if (request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        }

        var response = context.Response;
        if (!request.Headers.TryGetValue(AuthenticationConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            response.StatusCode = 401;
            await response.WriteAsync("API Key is missing");
            return;
        }

        var apiKey = config.Value.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            response.StatusCode = 500;
            await response.WriteAsync("API Key not configured");
            return;
        }

        if (!requestApiKey.Equals(apiKey))
        {
            response.StatusCode = 401;
            await response.WriteAsync("Invalid API Key");
            return;
        }

        await next(context);
    }
}