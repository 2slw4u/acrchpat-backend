using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<UserEntity> GetCurrentUser();

		AuthenticationResponse CreateAuthCredentials(UserEntity user);
	}
}
