using Microsoft.EntityFrameworkCore;
using PreferenceService.Database.TableModels;

namespace PreferenceService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Preference> Preferences { get; set; }
}