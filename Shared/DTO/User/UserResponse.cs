namespace DTO.User;

public record UserResponse(
    long Id,
    string Name,
    string Email,
    string? Location
);
