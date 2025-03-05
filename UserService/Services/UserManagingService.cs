using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class UserManagingService : IUserManagingService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<UserManagingService> _logger;
		private readonly IAuthenticationService _authenticationService;
		private readonly IRolesService _rolesService;
		private readonly IPasswordHasher<UserEntity> _passwordHasher;

		public UserManagingService(AppDbContext context, ILogger<UserManagingService> logger, IAuthenticationService authenticationService, IRolesService rolesService, IPasswordHasher<UserEntity> passwordHasher)
		{
			_context = context;
			_logger = logger;
			_authenticationService = authenticationService;
			_rolesService = rolesService;
			_passwordHasher = passwordHasher;
		}

		public async Task<AuthenticationResponse> Register(UserCreateDto newUserData)
		{

			await ValidateUserModel(newUserData);

			var newUser = await AddUserToDatabase(newUserData);

			var response = _authenticationService.CreateAuthCredentials(newUser);

			return response;
		}

		public async Task<AuthenticationResponse> CreateUser( UserCreateDto newUserData)
		{

			var user = await _authenticationService.Authenticate();

			if (!await IsEmployee(user))
			{
				throw new Exception("User isn't permitted to create new users");
			}

			await ValidateUserModel(newUserData);

			var newUser = await AddUserToDatabase(newUserData);

			var response = _authenticationService.CreateAuthCredentials(newUser);

			return response;
		}

		public async Task<AuthenticationResponse> Login(LoginDto data) 
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == data.Phone);

			if (user == null)
			{
				throw new ArgumentException("Invalid login credentials.");
			}

			var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, data.Password);
			if (verificationResult == PasswordVerificationResult.Failed)
			{
				throw new ArgumentException("Invalid login credentials.");
			}

			var response = _authenticationService.CreateAuthCredentials(user);

			return response;
		}

		public async Task<UserDto> GetUser() 
		{
			var user = await _authenticationService.Authenticate();
			_logger.LogInformation(user.ToString());

			var userDto = new UserDto { Email = user.Email, FullName = user.FullName, Phone = user.PhoneNumber, Roles = user.Roles.Select(r => new RoleDto { Name = r.Name, Id = r.Id }).ToList(), Id = user.Id, 
			IsBanned = user.Bans.Any(b => b.BanEnd == null)};

			return userDto;
		}

		public async Task<List<UserDto>> GetUsers(Guid? role)
		{
			IQueryable<UserEntity> query = _context.Users.Include(u => u.Roles).Include(u => u.Bans);

			if (role.HasValue)
			{
				query = query.Where(u => u.Roles.Any(r => r.Id == role.Value));
			}

			var users = await query.ToListAsync();

			var result = users.Select(u => new UserDto
			{
				Id = u.Id,
				FullName = u.FullName,
				Email = u.Email,
				Phone = u.PhoneNumber,
				IsBanned = u.Bans.Any(b => b.BanEnd == null),
				Roles = u.Roles.Select(r => new RoleDto
				{
					Id = r.Id,
					Name = r.Name
				}).ToList()
			});

			return result.ToList();
		}

		public async Task<UserPagedListDto> SearchUser(Guid? Id, RoleEntity[]? Roles, string? Name, string? Phone, string? Email) { throw new NotImplementedException(); }
		public async Task<Response> BanUser(Guid userId) { throw new NotImplementedException(); }
		public async Task<Response> UnbanUser(Guid userId) { throw new NotImplementedException(); }

		private async Task<bool> IsEmployee(UserEntity user)
		{
			return user.Roles.Contains(await _rolesService.FindByName("Employee"));
		}

		private async Task<UserEntity> AddUserToDatabase(UserCreateDto userDto)
		{
			var roles = await _context.Roles
				.Where(r => userDto.Roles.Contains(r.Id))
				.ToListAsync();

			var newUser = new UserEntity
			{
				Id = Guid.NewGuid(),
				FullName = userDto.FullName,
				PhoneNumber = userDto.PhoneNumber,
				Email = userDto.Email,
				Roles = roles
			};

			newUser.PasswordHash = _passwordHasher.HashPassword(newUser, userDto.Password);

			_context.Users.Add(newUser);

			await _context.SaveChangesAsync();

			return newUser;
		}

		private async Task ValidateUserModel(UserCreateDto user)
		{
			await ValidateDublicateField(user, "Email");
			await ValidateDublicateField(user, "PhoneNumber");

			var roles = user.Roles.All( r =>  _context.Roles.FirstOrDefault(role => role.Id == r) != null);
			if (!roles)
			{
				throw new ArgumentException($"Id does not match any in the available pull of roles");
			}
		}
		private async Task ValidateDublicateField(UserCreateDto user, string fieldName)
		{
			var propertyInfo = typeof(UserCreateDto).GetProperty(fieldName);
			if (propertyInfo == null)
			{
				throw new ArgumentException($"Field {fieldName} is not present in {nameof(UserCreateDto)}");
			}

			string fieldValue = (propertyInfo.GetValue(user) as string)?.ToLower().Trim();

			if (string.IsNullOrEmpty(fieldValue))
			{
				return;
			}

			bool exists = await _context.Users
				.AnyAsync(u => EF.Property<string>(u, fieldName).ToLower() == fieldValue);

			if (exists)
			{
				throw new ArgumentException($"User with {fieldName} '{fieldValue}' already exists");
			}
		}

	}
}
