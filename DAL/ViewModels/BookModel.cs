namespace DAL.ViewModels;

public class BookModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string PublishedYear { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public bool Status { get; set; } = true;
    public string? Image { get; set; }
    public string? IssuedBy { get; set; }
}