using DAL.Models;

namespace DAL.ViewModels;

public class AdminDashboardViewModal
{
    public List<Book>? Books { get; set; }
    public int TotalBooks { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}