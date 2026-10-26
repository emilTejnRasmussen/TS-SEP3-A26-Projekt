using System.ComponentModel.DataAnnotations;

namespace Entities;

public class User(string name, string email, string location)
{
    [Key]
    private long Id { get; set;}
    private string Name { get; set; } = name;
    private string Email { get; set; } = email;
    private string Location { get; set; } = location;
}