using Microsoft.OpenApi.Models;

namespace CodingChallenge.WebApi;

public sealed class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args);

        // Add services to the container.

        builder
            .Services
            .AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder
            .Services
            .AddEndpointsApiExplorer();
        builder
            .Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coupon", Version = "v1" });
                c.AddSecurityDefinition("ApiKey",
                    new OpenApiSecurityScheme
                    {
                        Description = "ApiKey must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "X-API-KEY",
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

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app.RunAsync();
    }
}