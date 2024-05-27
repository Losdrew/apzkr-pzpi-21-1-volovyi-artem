using Microsoft.AspNetCore.Identity;

namespace AutoCab.Server.Services;

public interface ITokenGenerator
{
    public Task<string> GenerateAsync(IdentityUser user);
}