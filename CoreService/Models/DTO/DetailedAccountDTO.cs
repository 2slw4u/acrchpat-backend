using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class DetailedAccountDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required] 
        public string Number { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public AccountStatus Status { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required]
        public CurrencyISO Currency { get; set; }
    }
}
