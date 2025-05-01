using LoanService.Database.TableModels;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Rate> Rates { get; set; }
    
    public DbSet<Loan> Loans { get; set; }
    
    public DbSet<LoanPayment> LoanPayments { get; set; }
    public DbSet<Request> Requests { get; set; }
}