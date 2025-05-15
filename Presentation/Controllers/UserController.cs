using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[CustomAuth("User")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        UserDashboardViewModal userDashboardViewModal = await _userService.GetUserDashboardViewModalAsync();
        return View(userDashboardViewModal);
    }

    [HttpGet]
    public async Task<IActionResult> RefreshBooksData(int pageIndex = 1, int pageSize = 8, bool inIssue = true, string searchValue = "", int filterValue = 0)
    {
        try
        {
            var userDashboardViewModal = await _userService.GetUserDashboardViewModalAsync(pageIndex, pageSize, inIssue, searchValue, filterValue);
            return PartialView("_BooksDataPartial", userDashboardViewModal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing books data.");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> IssueBook(int bookId)
    {
        try
        {
            return await _userService.IssueBookAsync(bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while issuing the book.");
            return Json(new { success = false, message = "An error occurred while issuing the book." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        try
        {
            return await _userService.ReturnBookAsync(bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning the book.");
            return Json(new { success = false, message = "An error occurred while returning the book." });
        }
    }
}