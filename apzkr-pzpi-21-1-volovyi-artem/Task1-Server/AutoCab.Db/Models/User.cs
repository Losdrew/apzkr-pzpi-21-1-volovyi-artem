namespace AutoCab.Db.Models;

public class User : Entity
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }
}