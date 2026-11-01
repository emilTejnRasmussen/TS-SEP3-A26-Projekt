namespace WebApp.Client.DTO.User;

// Matcher Shared/DTO/User/UserResponse.cs (id, name, email, location).
public record UserResponse(
    long Id,
    string Name,
    string Email,
    string? Location
);
