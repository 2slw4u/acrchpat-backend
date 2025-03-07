using LoanService.Models.General;
using LoanService.Models.Rate;

namespace LoanService.Models.Loan;

public class LoanDetailDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public RateDto Rate { get; set; }
    
    public LoanStatus Status { get; set; }
    
    public double GivenMoney { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public DateTime DeadlineTime { get; set; }
    
    public int TermDays { get; set; }
    
    public double DailyPayment { get; set; }
    
    public double TotalMoneyToPay { get; set; }
    
    public double MoneyLeftToPay { get; set; }
    
    public List<PaymentDto> Payments { get; set; }
    
    public List<TransactionDto> Transactions { get; set; }
}