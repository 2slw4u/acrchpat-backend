using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonitorService.Database.TableModels;

public class OperationResult
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string OperationName { get; set; } = "";
    [Required]
    public Guid TraceId { get; set; }
    [Required]
    public DateTime OperationStart { get; set; }
    [Required]
    public int ExecutionTime { get; set; }
    [Required]
    public bool IsSuccessful { get; set; }
    [Required]
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}