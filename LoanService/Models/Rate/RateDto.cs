namespace LoanService.Models.Rate;

public class RateDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public double YearlyRate { get; set; }
}