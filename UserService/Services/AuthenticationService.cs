using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserService.Database;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Exceptions;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly AppDbContext _context;
		private readonly IJwtService _jwtService;
		private readonly ILogger<AuthenticationService> _logger;

		public AuthenticationService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, ILogger<AuthenticationService> logger)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_jwtService = jwtService;
			_logger = logger;
		}

		public async Task<UserEntity> Authenticate()
		{
			var httpContext = _httpContextAccessor.HttpContext;
			if (httpContext == null || httpContext.User == null)
			{
				throw new UnauthorizedAccessException("Invalid token");
			}

			var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
			{
				throw new UnauthorizedAccessException("Invalid token");
			}

			var user = await _context.Users
				.Include(u => u.Roles)
				.Include(u => u.Bans)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
			{
				throw new UnauthorizedAccessException("Invalid token");
			}

			if (user.Bans.Any(b => b.BanEnd == null))
			{
				throw new ForbiddenException("User is banned");
			}

			return user;
		}

		public AuthenticationResponse CreateAuthCredentials(UserEntity user)
		{
			var token = _jwtService.GenerateToken(user);
			return new AuthenticationResponse { Auth = token };
		}
	}
}
