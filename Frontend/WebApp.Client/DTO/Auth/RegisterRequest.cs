using System.ComponentModel.DataAnnotations;

namespace WebApp.Client.DTO.Auth;

// Matcher Shared/DTO/Auth/RegisterRequest.cs (Name, Email, Password, Location).
// Klasse i stedet for record, så formularen kan binde til felterne.
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
