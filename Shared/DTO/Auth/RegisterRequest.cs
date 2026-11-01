using System.ComponentModel.DataAnnotations;

namespace DTO.Auth;

public record RegisterRequest(
    [Required]
    string Name,

    [Required, EmailAddress]
    string Email,

    [Required, MinLength(8)]
    string Password,

    string? Location
);