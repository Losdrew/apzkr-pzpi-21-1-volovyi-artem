namespace AutoCab.Shared.Errors.Base;

public class Error
{
    public List<ServiceError> ServiceErrors { get; set; } = new();

    public Error(List<ServiceError>? serviceErrors = null)
    {
        ServiceErrors = serviceErrors ?? ServiceErrors;
    }
}