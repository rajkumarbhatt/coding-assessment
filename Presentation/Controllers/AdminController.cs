using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly IAdminService _adminService;

    public AdminController(ILogger<AdminController> logger, IAdminService adminService)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet]
    [CustomAuth("Admin")]
    public IActionResult Dashboard()
    {
        try
        {
            var adminDashboardViewModal = _adminService.GetAdminDashboardViewModalAsync().Result;
            return View(adminDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching admin dashboard data.");
            Console.WriteLine(ex);
            return View("Error");
        }
    }

    [HttpPost]
    [CustomAuth("Admin")]
    public async Task<IActionResult> DeleteBook(int bookId)
    {
        try
        {
            return await _adminService.DeleteBookAsync(bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting the book.");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> RefreshBooksData (int pageIndex = 1, int pageSize = 8)
    {
        try
        {
            var adminDashboardViewModal = await _adminService.GetAdminDashboardViewModalAsync(pageIndex, pageSize);
            return PartialView("_BooksDataPartial", adminDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing books data.");
            return View("Error");
        }
    }
}