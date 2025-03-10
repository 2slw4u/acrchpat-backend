using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using UserService.Database;
using UserService.Integrations.AMQP.RabbitMQ;
using UserService.Integrations.AMQP.RabbitMQ.Producer;
using UserService.Middlewares;
using UserService.Models.Entities;
using UserService.Services;
using UserService.Services.Interfaces;
using UserService.Utils;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (!builder.Environment.IsDevelopment())
{
	builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
		RoleClaimType = ClaimTypes.Role
	};
});

builder.Services.AddSwaggerGen(option =>
{
	option.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "1.0" });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[]{}
					}
				});
});

builder.Services.AddScoped<IUserManagingService, UserManagingService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<UserBanStatusMessager>();
builder.Services.AddScoped<IBanService, BanService>();
builder.Services.AddSingleton<IRabbitMqProducerService, RabbitMqProducerService>();

var app = builder.Build();

using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
try
{
	AppDbContext context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
	DataSeeder.Seed(context);
}
catch (Exception ex)
{
	Console.WriteLine(ex);
}

app.UseExceptionMiddleware();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorizationMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();
