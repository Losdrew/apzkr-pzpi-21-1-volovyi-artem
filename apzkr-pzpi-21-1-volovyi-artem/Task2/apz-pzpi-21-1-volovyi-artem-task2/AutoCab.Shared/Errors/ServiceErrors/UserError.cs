using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class UserError : ServiceError
{
    public static UserError InvalidAuthorization = new()
    {
        Header = "Invalid authorization",
        ErrorMessage = "Invalid authorization",
        Code = 1
    };

    public static UserError UserNotFound = new()
    {
        Header = "User not found",
        ErrorMessage = "User not found",
        Code = 2
    };

    public static UserError ForbiddenAccess = new()
    {
        Header = "Access forbidden",
        ErrorMessage = "You don't have access",
        Code = 3
    };

    public static UserError GetUsersError = new()
    {
        Header = "Get user error",
        ErrorMessage = "Error when getting a list of users",
        Code = 4
    };

    public static UserError UserEditError = new()
    {
        Header = "Edit user error",
        ErrorMessage = "Error when editing a user",
        Code = 5
    };

    public static UserError UserDeleteError = new()
    {
        Header = "Delete user error",
        ErrorMessage = "Error when deleting a user",
        Code = 6
    };
}