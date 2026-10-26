using System.ComponentModel.DataAnnotations;

namespace Entities;

public class User(string name)
{
    [Key]
    private long Id { get; set;}
    private string Name { get; set; } = name;
    private string Email { get; set; } = name;
    private string Location { get; set; } = name;
}