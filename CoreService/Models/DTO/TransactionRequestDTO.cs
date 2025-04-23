using CoreService.Models.Enum;

namespace CoreService.Models.DTO
{
    public class TransactionRequestDTO
    {
        public Guid UserId { get; set; }
        public Guid? AccountId { get; set; }
        // используется для переводов
        public Guid? DestinationAccountId { get; set; }
        public Guid? LoanId { get; set; }
        public Guid? PaymentId { get; set; }
        public double Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
