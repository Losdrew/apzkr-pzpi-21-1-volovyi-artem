using Microsoft.AspNetCore.Identity;

namespace AutoCab.Server.Models.Account;

public class CreateIdentityUserResult
{
    public IdentityUser IdentityUser { get; set; }
    public Guid RoleId { get; set; }
}
