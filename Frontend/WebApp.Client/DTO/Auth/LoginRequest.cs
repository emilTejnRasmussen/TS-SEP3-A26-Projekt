using System.ComponentModel.DataAnnotations;

namespace WebApp.Client.DTO.Auth;

// Matcher Shared/DTO/Auth/LoginRequest.cs (Email, Password).
public class LoginRequest
{
    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Ugyldig email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password er påkrævet")]
    public string Password { get; set; } = "";
}
