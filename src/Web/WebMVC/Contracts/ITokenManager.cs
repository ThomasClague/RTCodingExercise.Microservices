namespace WebMVC.Contracts;

public interface ITokenManager
{
    Task SaveTokensAsync(string token, string refreshToken);
    Task ClearTokensAsync();
    Task<(string Token, string RefreshToken)> GetTokensAsync();
}