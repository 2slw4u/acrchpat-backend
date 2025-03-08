using UserService.Models.DTOs;

namespace UserService.Services.Interfaces
{
	public interface IBanService
	{
		Task<List<BanDto>> GetUserBanHistory(Guid id);
		Task<ResponseDto> BanUser(Guid id);
		Task<ResponseDto> UnbanUser(Guid id);
	}
}
