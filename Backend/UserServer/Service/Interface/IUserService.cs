using DTO.User;

namespace Service.Interface;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> GetUserByIdAsync(long id);
}