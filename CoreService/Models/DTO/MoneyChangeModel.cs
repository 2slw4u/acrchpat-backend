using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class MoneyChangeModel
    {
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Amount { get; set; }
    }
}
