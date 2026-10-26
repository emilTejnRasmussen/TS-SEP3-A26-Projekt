using DTO.User;
using Entities;

namespace Mapper;

public class UserMapper
{
    public static UserResponse ToDto(User user)
    {
        var response = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Location
        );

        return response;
    }
}