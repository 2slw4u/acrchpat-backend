using CoreService.Models.Request.Transaction;
using CoreService.Models.Response.Transaction;
using CoreService.Services;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("core/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        [HttpGet]
        [EndpointSummary("(GetTransactionsHistory) Returns history of user transactions")]
        [Authorize]
        public async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(GetTransactionsHistoryRequest request)
        {
            return await _transactionService.GetTransactionsHistory(HttpContext, request);
        }


        [HttpPost]
        [Route("{accountId}/deposit")]
        [EndpointSummary("(DepositMoneyToAccount) Imitates depositing money from ATM")]
        [Authorize]
        public async Task DepositMoneyToAccount(DepositMoneyToAccountRequest request)
        {
            await _transactionService.DepositMoneyToAccount(HttpContext, request);
        }

        [HttpPost]
        [Route("{accountId}/withdrawal")]
        [EndpointSummary("(WithdrawMoneyFromAccount) Imitates wihdrawal money with ATM")]
        [Authorize]
        public async Task WithdrawMoneyFromAccount(WithdrawMoneyFromAccountRequest request)
        {
            await _transactionService.WithdrawMoneyFromAccount(HttpContext, request);
        }
    }
}
