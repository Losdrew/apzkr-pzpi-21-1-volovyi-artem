using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class TripError : ServiceError
{
    public static TripError TripCreateError = new()
    {
        Header = "Create trip error",
        ErrorMessage = "Error when creating trip",
        Code = 1
    };

    public static TripError GetOwnTripsError = new()
    {
        Header = "Get own trips error",
        ErrorMessage = "Error when getting a list of user's trips",
        Code = 2
    };

    public static TripError TripNotFound = new()
    {
        Header = "Trip not found",
        ErrorMessage = "Trip not found",
        Code = 3
    };

    public static TripError TripCancelError = new()
    {
        Header = "Cancel trip error",
        ErrorMessage = "Error when cancelling trip",
        Code = 4
    };

    public static TripError TripUpdateError = new()
    {
        Header = "Update trip error",
        ErrorMessage = "Error when updating trip",
        Code = 5
    };

    public static TripError GetTripError = new()
    {
        Header = "Get trip error",
        ErrorMessage = "Error when getting trip",
        Code = 6
    };
}