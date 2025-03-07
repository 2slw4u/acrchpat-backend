namespace LoanService.Models.Loan;

public class PaymentDto
{
    public Guid Id { get; set; }
    
    public double Amount { get; set; }
    
    public DateTime PaymentTime { get; set; }
    
    public PaymentStatus Status { get; set; }
}