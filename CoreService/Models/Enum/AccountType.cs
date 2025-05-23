﻿using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<AccountType>))]
    public enum AccountType
    {
        [Description("Мастер-счет банка")]
        BankMasterAccount,
        [Description("Счет клиента, используемый для кредитов")]
        UserCreditAccount
    }
}
