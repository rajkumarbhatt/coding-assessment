using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class User
{
    public int UserId { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string Phone { get; set; } = null!;
    [Required]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}