using CoreService.Models.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Models.Database
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
    }
}
