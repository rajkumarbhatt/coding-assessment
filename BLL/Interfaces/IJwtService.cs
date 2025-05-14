using DAL.Models;

namespace BLL.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(User user, string role);
    }
}