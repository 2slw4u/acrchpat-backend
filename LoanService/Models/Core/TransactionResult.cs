namespace LoanService.Models.General;

public class TransactionResult
{
    public Guid TransactionId { get; set; }
    
    public Guid? LoanId { get; set; }
    
    public Guid? PaymentId { get; set; }
    
    public bool Status { get; set; }
    
    public TransactionType Type { get; set; }
    
    public string? ErrorMessage { get; set; }
}