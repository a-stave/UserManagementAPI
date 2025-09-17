using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }
}