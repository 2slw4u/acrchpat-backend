using System.ComponentModel.DataAnnotations;

namespace LoanService.Database.TableModels
{
    public class Request
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string IdempotencyKey { get; set; }
        [Required]
        public string OperationName { get; set; }
    }
}
