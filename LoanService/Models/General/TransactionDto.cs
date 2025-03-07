using System.ComponentModel.DataAnnotations;

namespace LoanService.Models.General;

public class TransactionDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Amount { get; set; }
    [Required]
    public TransactionType Type { get; set; }
    [Required]
    public DateTime PerformedAt { get; set; }
}