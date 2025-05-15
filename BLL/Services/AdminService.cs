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
        public async Task<AdminDashboardViewModal> GetAdminDashboardViewModalAsync(int pageIndex = 1, int pageSize = 8, bool inAvailable = true, string searchValue = "", int filterValue = 0)
        {
            string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(token);
            List<Book> books = new List<Book>();
            if (inAvailable)
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == true).OrderBy(b => b.BookId).ToListAsync();
            }
            else
            {
                books = await _context.Books.Where(b => b.IsDeleted == false && b.Status == false).OrderBy(b => b.BookId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchValue) || b.Author.ToLower().Contains(searchValue) || b.ISBN.Contains(searchValue) || b.PublishedYear.Contains(searchValue)).ToList();
            }

            switch(filterValue)
            {
                case 0:
                    books = books.OrderBy(b => b.BookId).ToList();
                    break;
                case 1:
                    books = books.OrderBy(b => b.Title).ToList();
                    break;
                case 2:
                    books = books.OrderByDescending(b => b.Title).ToList();
                    break;
                case 3:
                    books = books.OrderBy(b => b.Author).ToList();
                    break;
                case 4:
                    books = books.OrderByDescending(b => b.Author).ToList();
                    break;
                case 5:
                    books = books.OrderBy(b => b.PublishedYear).ToList();
                    break;
                case 6:
                    books = books.OrderByDescending(b => b.PublishedYear).ToList();
                    break;
                case 7:
                    books = books.OrderBy(b => b.ISBN).ToList();
                    break;
                case 8:
                    books = books.OrderByDescending(b => b.ISBN).ToList();
                    break;
                default:
                    break;
            }

            int totalBooks = books.Count;
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }
            if (totalPages == 0)
            {
                pageIndex = 1;
            }
           
            books = books.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            List<BookModel> bookModels = new List<BookModel>();
            foreach (var book in books)
            {
                BookModel bookModel = new BookModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Image = book.Image,
                    ISBN = book.ISBN,
                    PublishedYear = book.PublishedYear,
                    Status = book.Status,
                    IssuedBy = await _context.Users.Where(u => u.UserId == book.IssuedBy).Select(u => u.Name).FirstOrDefaultAsync() ?? "Not Issued"
                };
                bookModels.Add(bookModel);
            }
            AdminDashboardViewModal adminDashboardViewModal = new AdminDashboardViewModal
            {
                Books = bookModels,
                TotalBooks = totalBooks,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                AddEditBookViewModal = new AddEditBookViewModal(),
                InAvailable = inAvailable,
                Name = (await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId))?.Name
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
            if (!int.TryParse(addEditBookViewModal.PublishedYear, out int publishedYear) || publishedYear > DateTime.Now.Year || publishedYear < 1900)
            {
                return new JsonResult(new { success = false, message = "Invalid published year" });
            }
            if (addEditBookViewModal.BookId == 0)
            {
                if (await _context.Books.AnyAsync(b => b.Title.Trim().ToLower() == addEditBookViewModal.Title.Trim().ToLower() && b.IsDeleted == false))
                {
                    return new JsonResult(new { success = false, message = "Book with this title already exists" });
                }
                Book book = new Book
                {
                    Title = addEditBookViewModal.Title,
                    Author = addEditBookViewModal.Author,
                    PublishedYear = addEditBookViewModal.PublishedYear,
                    ISBN = addEditBookViewModal.ISBN,
                    Status = addEditBookViewModal.Status,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsDeleted = false
                };
                if (addEditBookViewModal.ImageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addEditBookViewModal.ImageFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/book-images", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await addEditBookViewModal.ImageFile.CopyToAsync(fileStream);
                    }
                    book.Image = fileName;
                }
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookId} added successfully by user with ID {UserId}", book.BookId, userId);
                return new JsonResult(new { success = true, message = "Book added successfully" });
            }
            else
            {
                if (await _context.Books.AnyAsync(b => b.Title.Trim().ToLower() == addEditBookViewModal.Title.Trim().ToLower() && b.BookId != addEditBookViewModal.BookId && b.IsDeleted == false))
                {
                    return new JsonResult(new { success = false, message = "Book with this title already exists" });
                }
                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == addEditBookViewModal.BookId && b.IsDeleted == false) ?? throw new Exception("Book not found");
                book.Title = addEditBookViewModal.Title;
                book.Author = addEditBookViewModal.Author;
                book.PublishedYear = addEditBookViewModal.PublishedYear;
                book.ISBN = addEditBookViewModal.ISBN;
                book.Status = addEditBookViewModal.Status;
                book.UpdatedBy = userId;
                book.IsDeleted = false;
                if (addEditBookViewModal.ImageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addEditBookViewModal.ImageFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/book-images", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await addEditBookViewModal.ImageFile.CopyToAsync(fileStream);
                    }
                    book.Image = fileName;
                }
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookId} updated successfully by user with ID {UserId}", book.BookId, userId);
                return new JsonResult(new { success = true, message = "Book updated successfully" });
            }
        }
    }
}