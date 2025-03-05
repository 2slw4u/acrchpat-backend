﻿using Microsoft.AspNetCore.Identity;
using UserService.Database;
using UserService.Models.Entities;

namespace UserService.Utils
{
	public static class DataSeeder
	{
		public static async Task Seed(AppDbContext context)
		{
			await context.Database.EnsureCreatedAsync();

			var passwordHasher = new PasswordHasher<UserEntity>();

			if (!context.Roles.Any() && !context.Users.Any())
			{
				var employeeRole = new RoleEntity { Id = Guid.NewGuid(), Name = "Employee" };
				var clientRole = new RoleEntity { Id = Guid.NewGuid(), Name = "Client" };

				context.Roles.AddRange(employeeRole, clientRole);
				context.SaveChanges();

				var employeeUser = new UserEntity
				{
					Id = Guid.NewGuid(),
					FullName = "Employee Default",
					PhoneNumber = "1234567890",
					Email = "employee@example.com",
					Roles = new List<RoleEntity> { employeeRole }
				};

				var clientUser = new UserEntity
				{
					Id = Guid.NewGuid(),
					FullName = "Client Default",
					PhoneNumber = "0987654321",
					Email = "client@example.com",
					Roles = new List<RoleEntity> { clientRole }
				};

				employeeUser.PasswordHash = passwordHasher.HashPassword(employeeUser, "password");
				clientUser.PasswordHash = passwordHasher.HashPassword(employeeUser, "password");

				context.Users.AddRange(employeeUser, clientUser);
				context.SaveChanges();
			}
		}
	}

}
