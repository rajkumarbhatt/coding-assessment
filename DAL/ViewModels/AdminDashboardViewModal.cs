using DAL.Models;

namespace DAL.ViewModels;

public class AdminDashboardViewModal
{
    public string? Name { get; set; }
    public List<BookModel>? Books { get; set; }
    public int TotalBooks { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool InAvailable { get; set; }
    public AddEditBookViewModal? AddEditBookViewModal { get; set; }
}