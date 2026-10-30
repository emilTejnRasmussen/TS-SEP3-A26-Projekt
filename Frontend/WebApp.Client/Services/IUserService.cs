using WebApp.Client.DTO.User;

namespace WebApp.Client.Services;

public interface IUserService
{
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<List<UserResponse>> GetAllAsync();
}
