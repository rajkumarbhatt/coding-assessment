using DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly DBContext _context;
        private readonly ILogger<LoginService> _logger;
        private readonly IJwtService _jwtService;

        public LoginService(DBContext context, IJwtService jwtService, ILogger<LoginService> logger)
        {
            _logger = logger;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<IActionResult> ValidateAsync(string email, string password)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} does not exist", email);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User does not exist"
                    });
                }
                if (password != user.Password)
                {
                    _logger.LogWarning("Invalid password for user with email {Email}", email);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Invalid password"
                    });
                }
                if (user != null && password == user.Password)
                {
                    var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == user.RoleId) ?? new Role();
                    string role = roleEntity.Name;
                    string token = await _jwtService.GenerateJwtTokenAsync(user, role);
                    _logger.LogInformation("User with email {Email} logged in successfully", email);
                    if (user.RoleId == 1)
                    {
                        return new JsonResult(new
                        {
                            isAdmin = true,
                            token = token,
                            success = true,
                            message = "Login successful"
                        });
                    } 
                    else if (user.RoleId == 2)
                    {
                        return new JsonResult(new
                        {
                            isAdmin = false,
                            token = token,
                            success = true,
                            message = "Login successful"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            isAdmin = false,
                            token = token,
                            success = false,
                            message = "Role not found"
                        });
                    }
                }
                _logger.LogWarning("Invalid credentials for user with email {Email}", email);
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid credentials"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating the user with email {Email}", email);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while validating the user",
                    error = ex.Message
                });
            }
        }
    }
}