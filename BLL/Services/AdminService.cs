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
        public async Task<AdminDashboardViewModal> GetAdminDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8, bool inAvailable = true)
        {
            List<Book> books = new List<Book>();
            if (inAvailable)
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == true).ToListAsync();
            }
            else
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == false).ToListAsync();
            }
            int totalBooks = books.Count;
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            books = books.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(b => b.BookId).ToList();
            AdminDashboardViewModal adminDashboardViewModal = new AdminDashboardViewModal
            {
                Books = books,
                TotalBooks = totalBooks,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                AddEditBookViewModal = new AddEditBookViewModal()
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
                _logger.LogInformation("Book with ID {BookId} deleted successfully by user with ID {UserId}", id, userId);
                return new JsonResult(new { success = true, message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the book with ID {BookId}", id);
                return new JsonResult(new { success = false, message = "Error occurred while deleting the book" });
            }
        }

        public async Task<AdminDashboardViewModal> GetBookByIdAsync(int id)
        {
            try
            {
                if (id == 0)
                {
                    return new AdminDashboardViewModal
                    {
                        AddEditBookViewModal = new AddEditBookViewModal()
                    };
                }
                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id && b.IsDeleted == false) ?? throw new Exception("Book not found");
                AdminDashboardViewModal adminDashboardViewModal = new AdminDashboardViewModal
                {
                    AddEditBookViewModal = new AddEditBookViewModal
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Author = book.Author,
                        Image = book.Image,
                        ISBN = book.ISBN,
                        PublishedYear = book.PublishedYear,
                        Status = book.Status,
                    }
                };
                return adminDashboardViewModal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the book with ID {BookId}", id);
                throw new Exception("Error occurred while fetching the book", ex);
            }
        }

        public async Task<IActionResult> AddEditBookAsync(AddEditBookViewModal addEditBookViewModal)
        {
            string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);
            if (addEditBookViewModal.BookId == 0)
            {
                Book book = new Book
                {
                    Title = addEditBookViewModal.Title,
                    Author = addEditBookViewModal.Author,
                    PublishedYear = addEditBookViewModal.PublishedYear,
                    ISBN = addEditBookViewModal.ISBN,
                    Status = addEditBookViewModal.Status,
                    Image = addEditBookViewModal.Image,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsDeleted = false
                };
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookId} added successfully by user with ID {UserId}", book.BookId, userId);
                return new JsonResult(new { success = true, message = "Book added successfully" });
            }
            else
            {
                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == addEditBookViewModal.BookId && b.IsDeleted == false) ?? throw new Exception("Book not found");
                book.Title = addEditBookViewModal.Title;
                book.Author = addEditBookViewModal.Author;
                book.PublishedYear = addEditBookViewModal.PublishedYear;
                book.ISBN = addEditBookViewModal.ISBN;
                book.Status = addEditBookViewModal.Status;
                // book.Image = addEditBookViewModal.Image;
                book.UpdatedBy = userId;
                book.IsDeleted = false;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookId} updated successfully by user with ID {UserId}", book.BookId, userId);
                return new JsonResult(new { success = true, message = "Book updated successfully" });
            }
        }
    }
}