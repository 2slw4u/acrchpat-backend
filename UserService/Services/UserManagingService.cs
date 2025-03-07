using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Exceptions;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class UserManagingService : IUserManagingService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<UserManagingService> _logger;
		private readonly IAuthenticationService _authenticationService;
		private readonly IRolesService _rolesService;
		private readonly UserManager<UserEntity> _userManager;

		public UserManagingService(
			AppDbContext context,
			ILogger<UserManagingService> logger,
			IAuthenticationService authenticationService,
			IRolesService rolesService,
			UserManager<UserEntity> userManager)
		{
			_context = context;
			_logger = logger;
			_authenticationService = authenticationService;
			_rolesService = rolesService;
			_userManager = userManager;
		}

		public async Task<AuthenticationResponse> Register(UserCreateDto newUserData)
		{
			await ValidateUserModel(newUserData);

			var newUser = await AddUserToDatabase(newUserData);

			var response = _authenticationService.CreateAuthCredentials(newUser);

			return response;
		}

		public async Task<AuthenticationResponse> CreateUser(UserCreateDto newUserData)
		{
			var user = await _authenticationService.Authenticate();

			if (!await IsEmployee(user))
			{
				throw new ForbiddenException("User isn't permitted to create new users");
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
				throw new BadRequestException("Invalid login credentials.");
			}

			var verificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, data.Password);
			if (verificationResult == PasswordVerificationResult.Failed)
			{
				throw new BadRequestException("Invalid login credentials.");
			}

			var response = _authenticationService.CreateAuthCredentials(user);

			return response;
		}

		public async Task<UserDto> GetUser()
		{
			var user = await _authenticationService.Authenticate();

			var userDto = new UserDto
			{
				Id = user.Id,
				Email = user.Email,
				FullName = user.FullName,
				Phone = user.PhoneNumber,
				Roles = user.Roles.Select(r => new RoleDto { Name = r.Name, Id = r.Id }).ToList(),
				IsBanned = user.Bans.Any(b => b.BanEnd == null)
			};

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
				Roles = u.Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }).ToList()
			});

			return result.ToList();
		}

		public async Task<UserPagedListDto> SearchUser(Guid? Id, RoleEntity[]? Roles, string? Name, string? Phone, string? Email)
		{
			throw new NotImplementedException();
		}

		private async Task<bool> IsEmployee(UserEntity user)
		{
			var employeeRole = await _rolesService.FindByName("Employee");
			return user.Roles.Contains(employeeRole);
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
				UserName = userDto.Email,
				Roles = roles
			};

			var result = await _userManager.CreateAsync(newUser, userDto.Password);
			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				throw new BadRequestException($"User creation failed: {errors}");
			}

			foreach (var role in roles)
			{
				var addToRoleResult = await _userManager.AddToRoleAsync(newUser, role.Name);
				if (!addToRoleResult.Succeeded)
				{
					var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
					throw new BadRequestException($"Adding to role '{role.Name}' failed: {errors}");
				}
			}

			return newUser;
		}

		private async Task ValidateUserModel(UserCreateDto user)
		{
			await ValidateDuplicateField(user, "Email");
			await ValidateDuplicateField(user, "PhoneNumber");

			var rolesExist = user.Roles.All(r => _context.Roles.FirstOrDefault(role => role.Id == r) != null);
			if (!rolesExist)
			{
				throw new BadRequestException("One or more role IDs do not match any available roles");
			}
		}

		private async Task ValidateDuplicateField(UserCreateDto user, string fieldName)
		{
			var propertyInfo = typeof(UserCreateDto).GetProperty(fieldName);
			if (propertyInfo == null)
			{
				throw new Exception($"Field {fieldName} is not present in {nameof(UserCreateDto)}");
			}

			string fieldValue = (propertyInfo.GetValue(user) as string)?.ToLower().Trim();

			if (string.IsNullOrEmpty(fieldValue))
			{
				return;
			}

			bool exists;
			if (fieldName == "Email")
			{
				exists = await _userManager.Users.AnyAsync(u => u.Email.ToLower() == fieldValue);
			}
			else if (fieldName == "PhoneNumber")
			{
				exists = await _userManager.Users.AnyAsync(u => u.PhoneNumber.ToLower() == fieldValue);
			}
			else
			{
				exists = await _context.Users.AnyAsync(u => EF.Property<string>(u, fieldName).ToLower() == fieldValue);
			}

			if (exists)
			{
				throw new BadRequestException($"User with {fieldName} '{fieldValue}' already exists");
			}
		}
	}
}
