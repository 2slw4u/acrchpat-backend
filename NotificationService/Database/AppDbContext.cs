using Microsoft.EntityFrameworkCore;
using NotificationService.Database;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<UserPushToken> UserPushTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserPushToken>()
         .Property(t => t.CreatedAt)
         .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}