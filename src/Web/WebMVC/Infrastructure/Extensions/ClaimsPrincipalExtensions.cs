using System.Security.Claims;
using Contracts.Constants;

namespace WebMVC.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(Roles.Admin);
        }
    }
} 