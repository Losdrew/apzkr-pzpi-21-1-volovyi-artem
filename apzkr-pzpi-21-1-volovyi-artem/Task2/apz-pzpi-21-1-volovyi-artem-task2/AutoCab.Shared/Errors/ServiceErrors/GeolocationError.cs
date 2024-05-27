using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class GeolocationError : ServiceError
{
    public static GeolocationError GetRouteError = new()
    {
        Header = "Get route error",
        ErrorMessage = "Error when getting route",
        Code = 1
    };
}