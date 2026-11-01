using WebApp.Client.DTO.User;

namespace WebApp.Client.Services;

// Brugere oprettes via IAuthService.RegisterAsync - backenden har ingen POST /Users.
public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
}
