﻿using CoreService.Attributes;
using CoreService.Models.Enum;
using CoreService.Models.Http.Request.Transaction;
using CoreService.Models.Http.Response.Transaction;
using CoreService.Services;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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
        [Route("history")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(GetTransactionsHistoryRequest request)
        {
            return await _transactionService.GetTransactionsHistory(HttpContext, request);
        }

        [HttpPost]
        [Route("{accountId}/deposit")]
        [EndpointSummary("(DepositMoneyToAccount) Imitates depositing money from ATM")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<DepositMoneyToAccountResponse> DepositMoneyToAccount(DepositMoneyToAccountRequest request)
        {
            return await _transactionService.DepositMoneyToAccount(HttpContext, request);
        }

        [HttpPost]
        [Route("{accountId}/withdrawal")]
        [EndpointSummary("(WithdrawMoneyFromAccount) Imitates wihdrawal money with ATM")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<WithdrawMoneyFromAccountResponse> WithdrawMoneyFromAccount(WithdrawMoneyFromAccountRequest request)
        {
            return await _transactionService.WithdrawMoneyFromAccount(HttpContext, request);
        }

        [HttpGet]
        [EndpointSummary("(GetTransactionData) Return basic data about requested transactions")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<GetTransactionsDataResponse> GetTransactionsData(GetTransactionsDataRequest request)
        {
            return await _transactionService.GetTransactionsData(HttpContext, request);
        }
    }
}
