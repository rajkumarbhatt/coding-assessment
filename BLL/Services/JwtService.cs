using DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BLL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> _logger;
        public JwtService(ILogger<JwtService> logger)
        {
            _logger = logger;
        }
        public async Task<string> GenerateJwtTokenAsync(User user, string role)
        {
            try
            {
                return await Task.Run(() =>
                {
                    List<Claim> claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, role),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test1232133454353533636gfhgfhxfdsfsdfsdfghgfhfghfghgfhfghfhfgh"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "http://localhost:5170",
                        audience: "http://localhost:5170",
                        claims: claims,
                        expires: DateTime.Now.AddMonths(1),
                        signingCredentials: creds);
                    _logger.LogInformation("JWT token generated successfully for user with email {UserId}", user.Email);
                    return new JwtSecurityTokenHandler().WriteToken(token);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the JWT token for user with email {UserId}", user.Email);
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while generating the JWT token", ex);
            }
        }
    }
}