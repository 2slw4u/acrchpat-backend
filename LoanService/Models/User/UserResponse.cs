namespace LoanService.Middleware;

public class UserResponse
{
    public Guid Id { get; set; }
    public List<RoleDto> Roles { get; set; }
}