using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDashboardViewModal> GetUserDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8, bool inIssue = true, string searchValue = "", int filterValue = 0);
        Task<IActionResult> IssueBookAsync(int bookId);
        Task<IActionResult> ReturnBookAsync(int bookId);
    }
}