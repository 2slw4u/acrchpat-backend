using Microsoft.EntityFrameworkCore;
using MonitorService.Database.TableModels;

namespace MonitorService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<OperationResult> OperationResults { get; set; }
}