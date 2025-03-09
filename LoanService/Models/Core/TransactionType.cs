using System.ComponentModel;

namespace LoanService.Models.General;

public enum TransactionType
{
    [Description("Withdrawal of funds through ATM")]
    Deposit,
    [Description("Deposit of funds through ATM")]
    Withdrawal,
    [Description("Money accrued from taking a loan")]
    LoanAccrual,
    [Description("Payment for a taken loan")]
    LoanPayment
}