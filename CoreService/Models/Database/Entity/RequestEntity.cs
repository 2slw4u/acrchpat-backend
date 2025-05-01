using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Database.Entity
{
    public class RequestEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string IdempotencyKey { get; set; }
        [Required]
        public string OperationName { get; set; }
    }
}
