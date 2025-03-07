using System.ComponentModel.DataAnnotations;
using LoanService.Models.Loan;

namespace LoanService.Database.TableModels;

public class Loan
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    public DateTime DeadlineTime { get; set; }

    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Rate Rate { get; set; }
    
    [Required]
    public LoanStatus Status { get; set; }
    
    [Required]
    public float GivenMoney { get; set; }
    
    [Required]
    public List<Guid> Transactions { get; set; }
}