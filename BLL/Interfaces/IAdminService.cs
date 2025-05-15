using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModal> GetAdminDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8, bool inAvailable = true);
        Task<IActionResult> DeleteBookAsync(int id);
        Task<IActionResult> AddEditBookAsync(AddEditBookViewModal addEditBookViewModal);
        Task<AdminDashboardViewModal> GetBookByIdAsync(int id);
    }
}