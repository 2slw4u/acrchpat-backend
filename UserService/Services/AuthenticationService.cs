using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserService.Database;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly AppDbContext _context;
		private readonly IJwtService _jwtService;

		public AuthenticationService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_jwtService = jwtService;
		}

		public async Task<UserEntity> Authenticate()
		{
			var userClaims = _httpContextAccessor.HttpContext?.User;
			if (userClaims == null)
			{
				throw new UnauthorizedAccessException("Invalid token");
			}

			var userIdClaim = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
			{
				return null;
			}
			var user = await _context.Users
				.Include(u => u.Roles)
				.Include(u => u.Bans)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user;
		}

		public AuthenticationResponse CreateAuthCredentials(UserEntity user)
		{
			var token = _jwtService.GenerateToken(user);
			var response = new AuthenticationResponse { Auth = token };

			return response;
		}
	}
}
