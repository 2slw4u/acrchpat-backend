using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.UniRate
{
    public class GetExchangeRatesResponse
    {
        [Required]
        public CurrencyISO BaseCurrency { get; set; }
        [Required]
        public CurrencyISO TargetCurrency { get; set; }
        [Required]
        public double ExchangeRate { get; set; }
    }
}
