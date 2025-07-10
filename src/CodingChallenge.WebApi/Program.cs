using CodingChallenge.WebApi.AuthenticationModule;
using CodingChallenge.WebApi.CouponModule;
using Microsoft.OpenApi.Models;

namespace CodingChallenge.WebApi;

public sealed class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args);

        // Add services to the container.
        var services = builder.Services;
        services.AddApiKeyAuthentication(builder.Configuration);
        services.AddCoupon(builder.Configuration);
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coupon", Version = "v1" });
                // TODO: Move ApiKey declaration to AuthenticationModule
                c.AddSecurityDefinition("ApiKey",
                    new OpenApiSecurityScheme
                    {
                        Description = "ApiKey must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = AuthenticationConstants.ApiKeyHeaderName,
                        In = ParameterLocation.Header,
                        Scheme = "ApiKeyScheme"
                    });
                var key = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement { { key, new List<string>() } };
                c.AddSecurityRequirement(requirement);
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApiKeyAuthentication();

        app.UseCoupon();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app.RunAsync();
    }
}