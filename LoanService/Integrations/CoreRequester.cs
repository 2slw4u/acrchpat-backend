using LoanService.Models.General;

namespace LoanService.Integrations;

public class CoreRequester : Requester
{
    protected override string ServiceName => "CoreService";
    private const string TransactionControllerName = "transaction";
    
    public CoreRequester(IConfiguration configuration, IHttpClientFactory httpClientFactory, HttpContextAccessor httpContextAccessor)
        : base(configuration, httpClientFactory, httpContextAccessor)
    {
    }
    
    public async Task<GetTransactionsDataResponse> GetTransactionsAsync(List<Guid>? transactionIds)
    {
        var endpointUrl = TransactionControllerName;
        if (transactionIds != null && transactionIds.Count != 0)
        {
            endpointUrl += $"?Transactions={transactionIds[0]}";
            for (int i = 1; i < transactionIds.Count; i++)
            {
                endpointUrl += $"&Transactions={transactionIds[i]}";
            }
        }
        
        return await GetAsync<GetTransactionsDataResponse>(endpointUrl);
    }
}