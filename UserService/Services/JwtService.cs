using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;
		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(UserEntity user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(90),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
