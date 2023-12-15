using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public TokenRepository(UserManager<User> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<string> CreateAccessToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!);
            var tokenExpires = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:DurationInDay"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.FullName!)
                }),
                Expires = tokenExpires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            var tokenString = jwtSecurityTokenHandler.WriteToken(securityToken);

            return tokenString;
        }
    }
}
