using System.ComponentModel.DataAnnotations;
using UserService.Models.DTOs;
using UserService.Models.Entities;

public class UserDto
{
	public UserDto() { }
	public UserDto(UserEntity u)
	{
		Id = u.Id;
		FullName = u.FullName;
		Email = u.Email;
		Phone = u.PhoneNumber;
		IsBanned = u.Bans.Any(b => b.BanEnd == null);
		Roles = u.Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }).ToList();
	}

	[Required]
	public Guid Id { get; set; }
	[Required]
	[MinLength(1)]
	public string FullName { get; set; }
	[Phone]
	[Required]
	public string Phone { get; set; }
	[EmailAddress]
	[Required]
	public string Email { get; set; }

	[Required]
	[MinLength(1)]
	public ICollection<RoleDto> Roles { get; set; }

	[Required]
	public bool IsBanned { get; set; }
}
