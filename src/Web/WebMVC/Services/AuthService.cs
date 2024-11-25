using System.Text.Json;
using WebMVC.Contracts;
using WebMVC.Models;
using WebMVC.Infrastructure;

namespace WebMVC.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IHttpClientFactory factory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger)
    {
        _factory = factory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(LoginViewModel loginData)
    {
        try
        {
            using var client = _factory.CreateClient(HttpClientConfig.IdentityClient);

            var response = await client.PostAsJsonAsync("api/Account/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                var token = result.GetProperty("token").GetString();

                var httpContext = _httpContextAccessor.HttpContext;
                httpContext?.Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(1)
                });

                return new AuthResult { Succeeded = true, Token = token };
            }

            _logger.LogWarning("Login failed for user {Email}", loginData.Email);
            return new AuthResult { Succeeded = false, Message = "Invalid username or password" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in user {Email}", loginData.Email);
            return new AuthResult { Succeeded = false, Message = "An error occurred during login" };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterViewModel registerData)
    {
        try
        {
            using var client = _factory.CreateClient(HttpClientConfig.IdentityClient);

            var response = await client.PostAsJsonAsync("api/account/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                return new AuthResult { Succeeded = true };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Registration failed: {ErrorContent}", errorContent);
            return new AuthResult { Succeeded = false, Message = "Registration failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user {Email}", registerData.Email);
            return new AuthResult { Succeeded = false, Message = "An error occurred during registration" };
        }
    }

    public async Task LogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        httpContext.Response.Cookies.Delete("jwt_token");
        httpContext.Response.Cookies.Delete("refresh_token");

        using var client = _factory.CreateClient(HttpClientConfig.IdentityClient);
        await client.PostAsync("api/account/logout", null);
    }
} 