using AutoCab.Server.Services;

namespace AutoCab.Server.BuildInjections;

internal static class ServicesInjection
{
    internal static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
        services.AddTransient<IGeolocationService, GeolocationService>();
    }
}