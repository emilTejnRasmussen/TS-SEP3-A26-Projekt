using Entities;

namespace Service.Interface;

public interface ITokenService
{
    string GenerateToken(User user);
}