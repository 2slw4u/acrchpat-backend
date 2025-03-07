namespace LoanService.Models.Loan;

public class LoanDetailDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid RateId { get; set; }
    
    public LoanStatus Status { get; set; }
    
    public float GivenMoney { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public DateTime DeadlineTime { get; set; }
    
    public int TermMonths { get; set; }
    
    public float MonthlyPayment { get; set; }
    
    public float TotalMoneyToPay { get; set; }
    
    public float MoneyLeftToPay { get; set; }
    
    public List<DateTime> RepaymentSchedule { get; set; }
    
    public List<Guid> Transactions { get; set; }
}