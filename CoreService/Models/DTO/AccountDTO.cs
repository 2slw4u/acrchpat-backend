using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class AccountDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public AccountStatus Status { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required]
        public CurrencyISO Currency { get; set; }
    }
}
