using LoanService.Database.TableModels;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Rate> Rates { get; set; }
}