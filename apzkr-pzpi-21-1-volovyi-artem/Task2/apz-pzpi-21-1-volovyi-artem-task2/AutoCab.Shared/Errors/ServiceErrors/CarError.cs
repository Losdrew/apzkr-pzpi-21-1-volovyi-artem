using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class CarError : ServiceError
{
    public static CarError CarCreateError = new()
    {
        Header = "Create car error",
        ErrorMessage = "Error when creating car",
        Code = 1
    };

    public static CarError CarNotFound = new()
    {
        Header = "Car not found",
        ErrorMessage = "Car not found",
        Code = 2
    };

    public static CarError CarUnavailableError = new()
    {
        Header = "Car unavailable",
        ErrorMessage = "Car is not available right now",
        Code = 3
    };

    public static CarError GetCarsError = new()
    {
        Header = "Get cars error",
        ErrorMessage = "Error when getting a list of cars",
        Code = 4
    };

    public static CarError CarEditError = new()
    {
        Header = "Edit car error",
        ErrorMessage = "Error when editing car",
        Code = 5
    };

    public static CarError CarDeleteError = new()
    {
        Header = "Delete car error",
        ErrorMessage = "Error when deleting car",
        Code = 6
    };
}