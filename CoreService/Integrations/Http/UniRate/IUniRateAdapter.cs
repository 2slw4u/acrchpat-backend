using CoreService.Models.Http.Request.UniRate;
using CoreService.Models.Http.Response.UniRate;

namespace CoreService.Integrations.Http.UniRate
{
    public interface IUniRateAdapter
    {
        Task<GetExchangeRatesResponse> GetExchangeRates(GetExchangeRatesRequest request);
        Task<ConvertCurrencyResponse> ConvertCurrency(ConvertCurrencyRequest request); 
    }
}
