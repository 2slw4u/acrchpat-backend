namespace LoanService.Models.General;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public AccountStatus Status { get; set; }
    public double Balance { get; set; }
}