using System.Diagnostics;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class HomeController : Controller
{
    
    private readonly ILogger<HomeController> _logger;
    private readonly ILoginService _loginService;

    public HomeController(ILogger<HomeController> logger, ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("api/validate")]
    public async Task<IActionResult> Validate([FromBody] LoginViewModel loginModel)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Invalid model state" });
        }
        return await _loginService.ValidateAsync(loginModel.Email, loginModel.Password);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult UnAuthorized()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PageNotFound()
    {
        return View();
    }
}
