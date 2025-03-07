namespace LoanService.Models.Loan;

public class LoanCreateModel
{
    public double Amount { get; set; }
    
    public int TermDays { get; set; }
    
    public Guid RateId { get; set; }
}