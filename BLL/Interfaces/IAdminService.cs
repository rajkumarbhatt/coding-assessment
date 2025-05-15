using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModal> GetAdminDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8);
        Task<IActionResult> DeleteBookAsync(int id);
    }
}