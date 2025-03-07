using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LoanService.Database.TableModels;

public class Rate
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MaxLength(5000)]
    public string Name { get; set; }
    
    [Required]
    [Range(0.1, 100, ErrorMessage = "Значение должно быть в диапазоне от {1} до {2}.")]
    public float RateValue { get; set; }
}