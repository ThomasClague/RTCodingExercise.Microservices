using Identity.API.Models;

namespace Identity.API.Contracts;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}