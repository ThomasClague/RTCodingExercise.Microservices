using WebMVC.Models;

namespace WebMVC.Contracts;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginViewModel loginData);
    Task<AuthResult> RegisterAsync(RegisterViewModel registerData);
    Task LogoutAsync();
}
