using System.ComponentModel.DataAnnotations;

namespace LoanService.Models.Rate;

public class RateCreateModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Name { get; set; }
    
    [Required]
    [Range(0.1, 100, ErrorMessage = "Значение должно быть в диапазоне от {1} до {2}.")]
    public double TwelveDayRate { get; set; }
}