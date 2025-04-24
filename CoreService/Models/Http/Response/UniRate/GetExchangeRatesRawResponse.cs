namespace CoreService.Models.Http.Response.UniRate
{
    public class GetExchangeRatesRawResponse
    {
        public string @base { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }
}
