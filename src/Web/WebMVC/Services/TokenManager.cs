using WebMVC.Contracts;

namespace WebMVC.Services.Auth;

public class TokenManager : ITokenManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SaveTokensAsync(string token, string refreshToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        httpContext?.Response.Cookies.Append("jwt_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddHours(1)
        });

        httpContext?.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(7)
        });
    }

    public async Task ClearTokensAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext?.Response.Cookies.Delete("jwt_token");
        httpContext?.Response.Cookies.Delete("refresh_token");
    }

    public async Task<(string Token, string RefreshToken)> GetTokensAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext?.Request.Cookies["jwt_token"];
        var refreshToken = httpContext?.Request.Cookies["refresh_token"];

        return (token, refreshToken);
    }
}