namespace DTO.User;

public record CreateUserRequest(
    string name,
    string password,
    string email,
    string? location
);