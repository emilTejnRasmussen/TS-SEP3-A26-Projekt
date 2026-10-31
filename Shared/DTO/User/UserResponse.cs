namespace DTO.User;

public record UserResponse(
    long id,
    string name,
    string email,
    string? location
);