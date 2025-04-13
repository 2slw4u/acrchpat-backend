using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class AccountCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
}
