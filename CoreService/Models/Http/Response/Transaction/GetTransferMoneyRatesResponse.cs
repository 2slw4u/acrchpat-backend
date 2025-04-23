using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Transaction
{
    public class GetTransferMoneyRatesResponse
    {
        [Required]
        public double Rate { get; set; }
        [Required] 
        public CurrencyISO Currency { get; set; }
    }
}
