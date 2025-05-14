using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class Role
{
    public int RoleId { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}