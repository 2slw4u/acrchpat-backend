using UserService.Models.DTOs;

namespace UserService.Services.Interfaces
{
	public interface IBanService
	{
		Task<List<BanDto>> GetUserBanHistory(Guid id);
		Task<Response> BanUser(Guid id);
		Task<Response> UnbanUser(Guid id);
	}
}
