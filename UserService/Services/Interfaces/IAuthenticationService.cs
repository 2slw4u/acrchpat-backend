using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<UserEntity> Authenticate();

		AuthenticationResponse CreateAuthCredentials(UserEntity user);
	}
}
