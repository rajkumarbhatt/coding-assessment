using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class AddEditBookViewModal
{
    public int BookId { get; set; }
    [Required (ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces.")]
    public string Title { get; set; } = null!;
    [Required (ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Author cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Author name can only contain letters and spaces.")]
    public string Author { get; set; } = null!;
    [Required (ErrorMessage = "Published year is required.")]
    [StringLength(4, ErrorMessage = "Published year must be 4 digits.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Published year must be a valid year.")]
    public string PublishedYear { get; set; } = null!;
    [Required (ErrorMessage = "ISBN No. is required.")]
    [StringLength(13, ErrorMessage = "ISBN must be 13 digits.")]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN must be a valid 13-digit number.")]
    public string ISBN { get; set; } = null!;
    [Required (ErrorMessage = "Status is required.")]
    public bool Status { get; set; } = true;
    public string? Image { get; set; }
}