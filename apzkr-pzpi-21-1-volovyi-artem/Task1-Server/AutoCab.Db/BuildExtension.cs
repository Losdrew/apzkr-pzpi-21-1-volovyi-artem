using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AutoCab.Db;

public static class BuildExtension
{
    public static void AddDbSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationString = configuration.GetRequiredSection("ConnectionString").Value;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configurationString);
        dataSourceBuilder.MapEnum<CarStatus>();
        dataSourceBuilder.MapEnum<TripStatus>();
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dataSource, builder =>
            {
                builder.MigrationsAssembly("AutoCab.Db");
                builder.UseNetTopologySuite();
            }));

        services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
    }
}