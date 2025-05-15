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

        public async Task<int> GetUserIdFromJwtTokenAsync(string token)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken == null)
                    {
                        throw new ArgumentException("Invalid JWT token");
                    }

                    var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
                    if (userIdClaim == null)
                    {
                        throw new ArgumentException("JWT token does not contain a user ID");
                    }

                    if (!int.TryParse(userIdClaim.Value, out int userId))
                    {
                        throw new ArgumentException("Invalid user ID in JWT token");
                    }

                    return userId;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while extracting the user ID from the JWT token");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while extracting the user ID from the JWT token", ex);
            }
        }
    }
}