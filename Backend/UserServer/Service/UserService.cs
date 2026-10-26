using DTO.User;
using Entities;
using Mapper;
using Repository;
using Service.Interface;

namespace Service;

public class UserService(IUserRepository userRepo) : IUserService
{
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User(
            request.name,
            request.email
        ) {
            Location = request.location ?? null
        };

        var createdUser = await userRepo.AddAsync(user);

        return UserMapper.ToDto(createdUser);
    }

    public Task<UserResponse> GetUserByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}