using System.ComponentModel.DataAnnotations;

namespace DTO.Auth;

// Klasse med set, så Blazor-formularer kan binde direkte til felterne.
public class LoginRequest
{
    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Ugyldig email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password er påkrævet")]
    public string Password { get; set; } = "";
}
