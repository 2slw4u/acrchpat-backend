using CoreService.Models.Enum;
using CoreService.Models.Exceptions;

namespace CoreService.Helpers
{
    public static class TransactionTypeMapper
    {
        public static FrontTransactionType MapTransactionTypeToFront(TransactionType transactionType, bool isDestinationAccount = false)
        {
            switch (transactionType)
            {
                case TransactionType.LoanAccrual:
                    return FrontTransactionType.LoanAccrual;
                case TransactionType.LoanPayment:
                    return FrontTransactionType.LoanPayment;
                case TransactionType.Deposit: 
                    return FrontTransactionType.Deposit;
                case TransactionType.Withdrawal:
                    return FrontTransactionType.Withdrawal;
                case TransactionType.Transfer:
                    if (isDestinationAccount) return FrontTransactionType.TransferTo;
                    else return FrontTransactionType.TransferFrom;
                default:
                    throw new InternalServerError("Не смогли смаппить типы транзакций");
            }
        }
    }
}
