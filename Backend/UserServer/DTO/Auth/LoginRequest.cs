using System.ComponentModel.DataAnnotations;

namespace DTO.Auth;

public record LoginRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    string Username,
    
    [Required] [MinLength(8)] 
    string Password
);