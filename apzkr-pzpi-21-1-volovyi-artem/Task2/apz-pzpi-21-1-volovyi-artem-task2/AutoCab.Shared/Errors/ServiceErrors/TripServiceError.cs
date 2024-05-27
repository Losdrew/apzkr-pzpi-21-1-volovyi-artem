using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class TripServiceError : ServiceError
{
    public static TripServiceError ServiceCreateError = new()
    {
        Header = "Create service error",
        ErrorMessage = "Error when creating service",
        Code = 1
    };

    public static TripServiceError ServiceNotFound = new()
    {
        Header = "Service not found",
        ErrorMessage = "Service not found",
        Code = 2
    };

    public static TripServiceError GetServicesError = new()
    {
        Header = "Get services error",
        ErrorMessage = "Error when getting a list of services",
        Code = 3
    };

    public static TripServiceError ServiceEditError = new()
    {
        Header = "Edit service error",
        ErrorMessage = "Error when editing service",
        Code = 4
    };

     public static TripServiceError ServiceDeleteError = new()
    {
        Header = "Delete service error",
        ErrorMessage = "Error when deleting service",
        Code = 5
    };
}