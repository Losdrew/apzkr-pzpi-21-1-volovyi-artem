using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class AccountError : ServiceError
{
    public static AccountError AccountCreateError = new()
    {
        Header = "Create account error",
        ErrorMessage = "Error when creating account",
        Code = 1
    };

    public static AccountError LoginServiceError = new()
    {
        Header = "Login error",
        ErrorMessage = "Error when performing login",
        Code = 2
    };
}