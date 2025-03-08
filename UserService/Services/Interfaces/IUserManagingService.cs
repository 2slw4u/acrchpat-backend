using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Services.Interfaces
{
	public interface IUserManagingService
	{
		Task<AuthenticationResponse> Register(UserCreateDto newUser);
		Task<AuthenticationResponse> Login(LoginDto data);
		Task<UserDto> GetUser();
		Task<List<UserDto>> GetUsers(Guid? role);
		Task<UserPagedListDto> SearchUser(Guid? Id, RoleEntity[]? Roles, string? Name, string? PhoneNumber, string? Email);
		Task<UserDto> CreateUser( UserCreateDto newUser);
		Task<ResponseDto> AddRole(Guid user, Guid role);
		Task<ResponseDto> RemoveRole(Guid user, Guid role);

	}
}
