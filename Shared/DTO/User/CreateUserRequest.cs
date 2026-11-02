namespace DTO.User;

public record CreateUserRequest(
    string Name,
    string Password,
    string Email,
    string? Location
);
