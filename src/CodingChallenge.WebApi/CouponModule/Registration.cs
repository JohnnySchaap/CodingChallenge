using CodingChallenge.WebApi.CouponModule.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.WebApi.CouponModule;

public static class Registration
{
    public static IServiceCollection AddCoupon(
        this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services.Configure<CouponConfig>(configuration.GetSection("Coupon"));
        services.AddDbContext<CouponDbContext>(opt => opt.UseSqlite(configuration.GetConnectionString("Coupon")));
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        return services;
    }

    public static IApplicationBuilder UseCoupon(this IApplicationBuilder app)
    {
        // TODO: Use DB migration
        using var scope = app.ApplicationServices.CreateScope();
        var couponDbContext = scope.ServiceProvider.GetRequiredService<CouponDbContext>();
        couponDbContext.Database.EnsureCreated();

        return app;
    }
}