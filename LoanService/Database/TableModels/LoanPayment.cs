using System.ComponentModel.DataAnnotations;
using LoanService.Models.Loan;

namespace LoanService.Database.TableModels;

public class LoanPayment
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime PaymentTime { get; set; }
    
    public PaymentStatus Status { get; set; }
    
    public double Amount { get; set; }
}