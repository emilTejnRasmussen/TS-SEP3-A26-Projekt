using System.ComponentModel.DataAnnotations;

namespace DTO.Auth;

public record LoginRequest(
    [Required, EmailAddress]
    string Email,

    [Required]
    string Password
);