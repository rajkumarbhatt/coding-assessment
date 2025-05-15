using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[CustomAuth("Admin")]
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
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var adminDashboardViewModal = await _adminService.GetAdminDashboardViewModalAsync();
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
    public async Task<IActionResult> RefreshBooksData(int pageIndex = 1, int pageSize = 8, bool inAvailable = true, string searchValue = "", int filterValue = 0)
    {
        try
        {
            var adminDashboardViewModal = await _adminService.GetAdminDashboardViewModalAsync(pageIndex, pageSize, inAvailable, searchValue, filterValue);
            return PartialView("_BooksDataPartial", adminDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing books data.");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBookById(int bookId)
    {
        try
        {
            AdminDashboardViewModal adminDashboardViewModal = await _adminService.GetBookByIdAsync(bookId);
            return PartialView("_AddEditBookModalPartial", adminDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching book data.");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEditBook([FromForm] AddEditBookViewModal addEditBookViewModal)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }
            return await _adminService.AddEditBookAsync(addEditBookViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating the book.");
            return Json(new { success = false, message = "Error occurred while updating the book" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> RefreshAddEditBook()
    {
        try
        {
            AdminDashboardViewModal adminDashboardViewModal = await _adminService.GetBookByIdAsync(0);
            return PartialView("_AddEditBookModalPartial", adminDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching book data.");
            return View("Error");
        }
    }
}