using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Book
{
    public int BookId { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string Author { get; set; } = null!;
    [Required]
    [StringLength(4)]
    public string PublishedYear { get; set; } = null!;
    [Required]
    [StringLength(13)]
    public string ISBN { get; set; } = null!;
    [Required]
    public bool Status { get; set; } = true;
    public string? Image { get; set; }
    public int CreatedBy { get; set; }
    public int UpdatedBy { get; set; }
    public int IssuedBy { get; set; }
    [Required]
    public bool IsDeleted { get; set; } = false;
    public virtual User CreatedByNavigation { get; set; } = null!;
    public virtual User UpdatedByNavigation { get; set; } = null!;
}