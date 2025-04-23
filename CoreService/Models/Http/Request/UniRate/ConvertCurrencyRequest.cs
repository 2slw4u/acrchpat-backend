using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.UniRate
{
    public class ConvertCurrencyRequest
    {
        [Required]
        public CurrencyISO BaseCurrency { get; set; }
        [Required]
        public CurrencyISO TargetCurrency { get; set; }
        [Required]
        public double Amount { get; set; }
    }
}
