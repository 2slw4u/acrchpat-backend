using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Integrations.AMQP.RabbitMQ.Producer;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Exceptions;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class BanService : IBanService
	{
		private readonly AppDbContext _context;
		private readonly IAuthenticationService _authenticationService;
		private readonly IUserManagingService _userManagingService;
		private readonly IRabbitMqProducerService _rabbitMqProducerService;
		private readonly ILogger<BanService> _logger;

		public BanService(AppDbContext context, IAuthenticationService authenticationService, ILogger<BanService> logger, IRabbitMqProducerService rabbitMqProducerService, IUserManagingService userManagingService)
		{
			_context = context;
			_authenticationService = authenticationService;
			_logger = logger;
			_rabbitMqProducerService = rabbitMqProducerService;
			_userManagingService = userManagingService;
		}

		public async Task<List<BanDto>> GetUserBanHistory(Guid id)
		{
			var bans = await _context.Bans
				.Include(b => b.BannedBy)
				.Include(b => b.BannedUser)
				.Where(b => b.BannedUser.Id == id)
				.OrderByDescending(b => b.BanStart)
				.ToListAsync();

			var result = bans.Select(b => new BanDto
			{
				Id = b.Id,
				BannedBy = b.BannedBy.Id,
				BannedUser = b.BannedUser.Id,
				BanStart = b.BanStart,
				BanEnd = b.BanEnd
			}).ToList();

			return result;
		}

		public async Task<ResponseDto> BanUser(Guid id)
		{
			var currentUser = await _authenticationService.GetCurrentUser();

			var userToBan = await _userManagingService.FullInfoById(id);

			if (currentUser.Id == id)
			{
				throw new BadRequestException("Banning oneself is not permitted.");
			}

			bool isAlreadyBanned = await _context.Bans.AnyAsync(b => b.BannedUser.Id == id && b.BanEnd == null);
			if (isAlreadyBanned)
			{
				throw new BadRequestException("User is already banned");
			}

			var ban = new BanEntity
			{
				Id = Guid.NewGuid(),
				BannedBy = currentUser,
				BannedUser = userToBan,
				BanStart = DateTime.UtcNow,
				BanEnd = null
			};

			_context.Bans.Add(ban);
			await _context.SaveChangesAsync();

			await SendUpdate(userToBan);

			var result = new ResponseDto
			{
				Status = "Success",
				Message = "User banned successfully."
			};
			return result;
		}

		public async Task<ResponseDto> UnbanUser(Guid id)
		{
			var currentUser = await _authenticationService.GetCurrentUser();

			var userToUnban = await _userManagingService.FullInfoById(id);

			if (currentUser.Id == id)
			{
				throw new BadRequestException("Unbanning oneself is not permitted.");
			}

			var activeBan = await _context.Bans
				.Where(b => b.BannedUser.Id == id && b.BanEnd == null)
				.FirstOrDefaultAsync();

			if (activeBan == null)
			{
				throw new BadRequestException("User is not banned");
			}

			activeBan.BanEnd = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			await SendUpdate(userToUnban);

			var result = new ResponseDto
			{
				Status = "Success",
				Message = "User unbanned successfully."
			};

			return result;
		}

		private async Task SendUpdate(UserEntity user)
		{
			await _rabbitMqProducerService.SendUserBanStatusUpdateMessage(new UserBanStatusUpdateDto(user));
		}
	}
}

