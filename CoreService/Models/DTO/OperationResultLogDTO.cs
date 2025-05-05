using CoreService.Models.Exceptions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class OperationResultLogDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string OperationName { get; set; }
        [Required]
        public Guid TraceId { get; set; }
        [Required]
        [Description("Время начала выполнения операции в UTC")]
        public DateTime OperationStart { get; set; }
        [Required]
        [Description("Время выполнения в секундах")]
        public int ExecutionTime { get; set; }
        [Required]
        public bool IsSuccessful { get; set; }
        [Required]
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
