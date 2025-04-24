namespace CoreService.Models.Http.Response.UniRate
{
    public class ConvertCurrencyRawResponse
    {
        public double amount { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double result { get; set; }
    }
}
