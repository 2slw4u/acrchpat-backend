using CoreService.Models.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Models.Database
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>()
                .HasIndex(a => a.Number)
                .IsUnique();

            modelBuilder.Entity<AccountEntity>().HasData(
                new AccountEntity()
                {
                    Id = Guid.Parse("C80C02E2-AF14-4EA7-B021-49372536D995"),
                    UserId = Guid.Empty,
                    Name = "MASTER_ACCOUNT",
                    Balance = 100000,
                    Type = Enum.AccountType.BankMasterAccount,
                    Status = Enum.AccountStatus.Opened,
                    Currency = Enum.CurrencyISO.RUB,
                    CreatedAt = DateTime.MinValue,
                    Number = "1"
                }
                );
        }
    }
}
