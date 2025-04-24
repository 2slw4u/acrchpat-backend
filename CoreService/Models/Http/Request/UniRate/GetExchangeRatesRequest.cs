using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.UniRate
{
    public class GetExchangeRatesRequest
    {
        [Required]
        public CurrencyISO BaseCurrency { get; set; }
        [Required]
        public CurrencyISO TargetCurrency { get; set; }
    }
}
