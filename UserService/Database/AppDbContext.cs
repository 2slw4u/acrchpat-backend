using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Models.Entities;

namespace UserService.Database
{
	public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<BanEntity> Bans { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<BanEntity>()
				.HasOne(b => b.BannedBy)
				.WithMany()
				.HasForeignKey("BannedById")
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<BanEntity>()
				.HasOne(b => b.BannedUser)
				.WithMany(u => u.Bans)
				.HasForeignKey("BannedUserId")
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<UserEntity>()
				.HasMany(u => u.Roles)
				.WithMany()
				.UsingEntity<Dictionary<string, object>>(
					"UserRoles",
					r => r.HasOne<RoleEntity>()
						  .WithMany()
						  .HasForeignKey("RoleId")
						  .OnDelete(DeleteBehavior.Cascade),
					u => u.HasOne<UserEntity>()
						  .WithMany()
						  .HasForeignKey("UserId")
						  .OnDelete(DeleteBehavior.Cascade),
					je =>
					{
						je.HasKey("UserId", "RoleId");
					});
		}
	}
}