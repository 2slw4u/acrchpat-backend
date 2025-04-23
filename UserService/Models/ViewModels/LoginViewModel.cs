using System.ComponentModel.DataAnnotations;

namespace UserService.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}
