namespace LoanService.Models.General;

public class TransactionDto
{
    public Guid Id { get; set; }
    
    public double Amount { get; set; }
    
    public TransactionType Type { get; set; }
    
    public DateTime PerformedAt { get; set; }
}