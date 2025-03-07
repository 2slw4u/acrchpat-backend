namespace UserService.Models.DTOs
{
	public class UserPagedListDto
	{
		public List<UserDto>? Users { get; set; }
		public PageInfoModel Pagination { get; set; }
	}
}
