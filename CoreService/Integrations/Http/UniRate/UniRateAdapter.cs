using CoreService.Helpers;
using CoreService.Models.Exceptions;
using CoreService.Models.Http.Request.UniRate;
using CoreService.Models.Http.Response.UniRate;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CoreService.Integrations.Http.UniRate
{
    public class UniRateAdapter : IUniRateAdapter
    {
        public async Task<GetExchangeRatesResponse> GetExchangeRates(GetExchangeRatesRequest request)
        {
            try
            {
                HttpClient client = new HttpClient();
                var operationUrl = ConfigurationHelper._config["Integrations:Http:UniRate:GetExchangeRatesOperationRoute"];
                var fullUrl = GetFullOperationUrl(operationUrl);
                var queryParams = new List<KeyValuePair<string, string?>>
                {
                    new("api_key", GetApiKey()),
                    new("from", request.BaseCurrency.ToString()),
                };
                fullUrl = QueryHelpers.AddQueryString(fullUrl, queryParams);
                var uniRateResponse = await client.GetAsync(fullUrl);
                if (!uniRateResponse.IsSuccessStatusCode)
                {
                    throw new UniRateError(await uniRateResponse.Content.ReadAsStringAsync());
                }

                var responseBody = await uniRateResponse.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<GetExchangeRatesRawResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null)
                {
                    throw new UniRateError();
                }

                return new GetExchangeRatesResponse
                {
                    BaseCurrency = request.BaseCurrency,
                    TargetCurrency = request.TargetCurrency,
                    ExchangeRate = data.rates.Where(x => x.Key == request.TargetCurrency.ToString()).FirstOrDefault().Value
                };
            }
            catch (Exception ex)
            {
                throw new UniRateError(ex.Message);
            }
        }
        public async Task<ConvertCurrencyResponse> ConvertCurrency(ConvertCurrencyRequest request)
        {
            try
            {
                HttpClient client = new HttpClient();
                var operationUrl = ConfigurationHelper._config["Integrations:Http:UniRate:ConvertCurrencyOperationRoute"];
                var fullUrl = GetFullOperationUrl(operationUrl);
                var queryParams = new List<KeyValuePair<string, string?>>
                {
                    new("api_key", GetApiKey()),
                    new("amount", request.Amount.ToString()),
                    new("from", request.BaseCurrency.ToString()),
                    new("to", request.TargetCurrency.ToString()),
                };
                fullUrl = QueryHelpers.AddQueryString(fullUrl, queryParams);
                var uniRateResponse = await client.GetAsync(fullUrl);
                if (!uniRateResponse.IsSuccessStatusCode)
                {
                    throw new UniRateError(await uniRateResponse.Content.ReadAsStringAsync());
                }

                var responseBody = await uniRateResponse.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<ConvertCurrencyRawResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null)
                {
                    throw new UniRateError();
                }

                return new ConvertCurrencyResponse
                {
                    BaseCurrency = request.BaseCurrency,
                    TargetCurrency = request.TargetCurrency,
                    Amount = data.result
                };
            }
            catch (Exception ex)
            {
                throw new UniRateError(ex.Message);
            }
        }
        private string GetFullOperationUrl(string operation)
        {
            var serviceUrl = ConfigurationHelper._config["Integrations:Http:UniRate:Api"];
            return $"{serviceUrl}{operation}";
        }

        private string GetApiKey()
        {
            var apiKey = ConfigurationHelper._config["Integrations:Http:UniRate:ApiKey"];
            return apiKey;
        }

    }
}
