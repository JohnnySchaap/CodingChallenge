namespace CodingChallenge.WebApi.AuthenticationModule;

public static class Registration
{
    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services.Configure<AuthenticationConfig>(configuration.GetSection("Authentication"));
        return services;
    }

    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}