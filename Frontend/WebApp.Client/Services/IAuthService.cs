using WebApp.Client.DTO.Auth;

namespace WebApp.Client.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task LogoutAsync();
}
