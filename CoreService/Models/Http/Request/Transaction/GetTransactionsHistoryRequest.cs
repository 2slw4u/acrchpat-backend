using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class GetTransactionsHistoryRequest
    {
        [FromQuery]
        public List<Guid>? Accounts { get; set; }
    }
}
