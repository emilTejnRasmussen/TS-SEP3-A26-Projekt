using System.ComponentModel.DataAnnotations;

namespace DTO.Auth;

// Klasse med set, så Blazor-formularer kan binde direkte til felterne.
public class RegisterRequest
{
    [Required(ErrorMessage = "Navn er påkrævet")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Ugyldig email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password er påkrævet")]
    [MinLength(8, ErrorMessage = "Password skal være mindst 8 tegn")]
    public string Password { get; set; } = "";

    public string? Location { get; set; }
}
