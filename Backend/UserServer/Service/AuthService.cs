using DTO.Auth;
using Entities;
using Exceptions;
using Microsoft.AspNetCore.Identity;
using Repository;
using Repository.Exceptions;
using Service.Interface;
using Services.Exceptions;

namespace Service;

public class AuthService(
    IUserRepository userRepo,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : IAuthService
{ 
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        await EnsureEmailDoesNotExistAsync(request.Email);

        var user = new User(request.Name, request.Email);

        user.HashedPassword = passwordHasher.HashPassword(user, request.Password);
        user.Location = request.Location;

        var createdUser = await userRepo.AddAsync(user);

        var token = tokenService.GenerateToken(createdUser);

        return new AuthResponse(token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User user;
        try
        {
            user = await userRepo.GetByEmailAsync(request.Email);
        }
        catch (UserNotFoundException)
        {
            throw new InvalidCredentialsException();
        }

        EnsureValidPassword(user, request.Password);

        var token = tokenService.GenerateToken(user);

        return new AuthResponse(token);
    }

    private void EnsureValidPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.HashedPassword,
            password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }
    }
    
    private async Task EnsureEmailDoesNotExistAsync(string username)
    {
        var usernameAlreadyExists = await userRepo.EmailAlreadyExistsAsync(username);

        if (usernameAlreadyExists)
        {
            throw new EmailAlreadyExistsException(username);
        }
    }
}
