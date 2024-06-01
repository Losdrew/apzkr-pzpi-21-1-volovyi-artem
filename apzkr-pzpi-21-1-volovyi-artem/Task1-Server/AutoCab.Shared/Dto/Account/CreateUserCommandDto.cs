namespace AutoCab.Shared.Dto.Account;

public class CreateUserCommandDto : CredentialsDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
}