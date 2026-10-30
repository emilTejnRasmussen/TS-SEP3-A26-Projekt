using System.ComponentModel.DataAnnotations;

namespace WebApp.Client.DTO.User;

// Matcher Backend/UserServer/DTO/User/CreateUserRequest.cs (name, password, email, location).
// Klasse i stedet for record, så formularen kan binde til felterne.
public class CreateUserRequest
{
    [Required(ErrorMessage = "Navn er påkrævet")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Password er påkrævet")]
    [MinLength(6, ErrorMessage = "Password skal være mindst 6 tegn")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Ugyldig email")]
    public string Email { get; set; } = "";

    public string? Location { get; set; }
}
