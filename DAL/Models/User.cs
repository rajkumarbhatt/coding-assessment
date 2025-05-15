using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class User
{
    public int UserId { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string Password { get; set; } = null!;
    [Required]
    [StringLength(10)]
    public string Phone { get; set; } = null!;
    public int NoOfBookIssued { get; set; }
    [Required]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Book> BookCreatedByNavigations { get; set; } = new List<Book>();
    public virtual ICollection<Book> BookUpdatedByNavigations { get; set; } = new List<Book>();
}