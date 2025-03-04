using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class AccountChangeModel
    {
        [Required]
        public string Name { get; set; }
    }
}
