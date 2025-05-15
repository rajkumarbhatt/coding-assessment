using BLL.Interfaces;
using DAL.DbContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly DBContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminService(DBContext context, IJwtService jwtService, ILogger<LoginService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtService = jwtService;
            _context = context;
        }
        public async Task<AdminDashboardViewModal> GetAdminDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8)
        {
            List<Book> books = await _context.Books.Where(b => b.IsDeleted == false).ToListAsync();
            int totalBooks = books.Count;
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            books = books.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            AdminDashboardViewModal adminDashboardViewModal = new AdminDashboardViewModal
            {
                Books = books,
                TotalBooks = totalBooks,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages
            };
            return adminDashboardViewModal;
        }

        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            try
            {
                string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
                int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);
                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id && b.IsDeleted == false) ?? throw new Exception("Book not found");
                book.IsDeleted = true;
                book.UpdatedBy = userId;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the book with ID {BookId}", id);
                return new JsonResult(new { success = false, message = "Error occurred while deleting the book" });
            }
        }
    }
}