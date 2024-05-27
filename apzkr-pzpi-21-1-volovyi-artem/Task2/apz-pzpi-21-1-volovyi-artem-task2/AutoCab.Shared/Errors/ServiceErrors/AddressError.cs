using AutoCab.Shared.Errors.Base;

namespace AutoCab.Shared.Errors.ServiceErrors;

public class AddressError : ServiceError
{
    public static AddressError GetAddressesError = new()
    {
        Header = "Get addresses error",
        ErrorMessage = "Error when getting a list of addresses",
        Code = 1
    };
}