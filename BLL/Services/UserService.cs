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
    public class UserService : IUserService
    {
        private readonly DBContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(DBContext context, IJwtService jwtService, ILogger<LoginService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtService = jwtService;
            _context = context;
        }
        public async Task<UserDashboardViewModal> GetUserDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8, bool inIssue = true)
        {
            string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);
            List<Book> books = new List<Book>();
            if (inIssue)
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == true).ToListAsync();
            }
            else
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == false && b.IssuedBy == userId).ToListAsync();
            }
            int totalBooks = books.Count;
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            books = books.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(b => b.BookId).ToList();
            UserDashboardViewModal userDashboardViewModal = new UserDashboardViewModal
            {
                Books = books,
                TotalBooks = totalBooks,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                InIssue = inIssue
            };
            return userDashboardViewModal;
        }

        public async Task<IActionResult> IssueBookAsync(int bookId)
        {
            try
            {
                string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
                int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);

                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new Exception("User not found");

                if (user.NoOfBookIssued >= 3)
                {
                    return new JsonResult(new { success = false, message = "You have already issued 3 books, please return one to issue another." });
                }

                user.NoOfBookIssued += 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId && b.IsDeleted == false) ?? throw new Exception("Book not found");
                book.Status = false;
                book.IssuedBy = userId;
                book.UpdatedBy = userId;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Book with ID {BookId} issued successfully to user with ID {UserId}", bookId, userId);
                return new JsonResult(new { success = true, message = "Book issued successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while issuing the book.");
                return new JsonResult(new { success = false, message = "An error occurred while issuing the book." });
            }
        }

        public async Task<IActionResult> ReturnBookAsync(int bookId)
        {
            try
            {
                string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
                int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);

                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new Exception("User not found");

                if (user.NoOfBookIssued <= 0)
                {
                    return new JsonResult(new { success = false, message = "You have no books issued." });
                }

                user.NoOfBookIssued -= 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId && b.IsDeleted == false) ?? throw new Exception("Book not found");
                book.Status = true;
                book.IssuedBy = 0;
                book.UpdatedBy = userId;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book with ID {BookId} returned successfully by user with ID {UserId}", bookId, userId);
                return new JsonResult(new { success = true, message = "Book returned successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while returning the book.");
                return new JsonResult(new { success = false, message = "An error occurred while returning the book." });
            }
        }
    }
}