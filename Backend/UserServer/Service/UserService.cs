using DTO.User;
using Entities;
using Mapper;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Interface;

namespace Service;

public class UserService(IUserRepository userRepo) : IUserService
{
    public async Task<UserResponse> GetUserByIdAsync(long id)
    {
        var user = await userRepo.GetSingleByIdAsync(id);

        return UserMapper.ToDto(user);
    }

    public async Task<List<UserResponse>> GetManyAsync(string? nameContains = null)
    {
        var users = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            users = users.Where(u => EF.Functions.ILike(u.Name, $"%{nameContains}%"));
        }

        var result = await users.ToListAsync();

        return [.. result.Select(UserMapper.ToDto)];
    }
}