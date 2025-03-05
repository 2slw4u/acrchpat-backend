using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models.Entities;

namespace UserService.Services.Interfaces
{
	public interface IJwtService
	{
		string GenerateToken(UserEntity user);
	}

}
