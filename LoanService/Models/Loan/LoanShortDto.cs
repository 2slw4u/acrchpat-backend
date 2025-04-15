namespace LoanService.Models.Loan;

public class LoanShortDto
{
    public Guid Id { get; set; }
    
    public double Amount { get; set; }
    
    public double Rate { get; set; }
    
    public DateTime DeadlineTime { get; set; }
    
    public LoanStatus Status { get; set; }
}