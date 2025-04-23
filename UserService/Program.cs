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
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Duende.IdentityServer;

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

builder.Services.AddControllersWithViews();
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

builder.Services.AddIdentityServer(options =>
{
    options.UserInteraction.LoginUrl = "/Account/Login";
    options.UserInteraction.LoginReturnUrlParameter = "returnUrl";
	options.UserInteraction.LogoutUrl = "/Account/Logout";
	options.UserInteraction.LogoutIdParameter = "logoutId";

})
    .AddAspNetIdentity<UserEntity>()
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource("api1", "My API")
        {
            Scopes = { "api1" }
        }
    })
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new ApiScope("api1", "My API")
    })
	.AddInMemoryClients(new List<Client>
	{
		new Client
		{
			ClientId = "client_app",
			AllowedGrantTypes = GrantTypes.Code,
			RequireClientSecret = false,
			RequirePkce = true,
			RedirectUris = { "http://localhost:5173/signin-callback" },
			PostLogoutRedirectUris = { "http://localhost:5173/" },
			AllowedCorsOrigins = { "http://localhost:5173" },
			AllowedScopes = { "openid", "profile", "api1" }
		},
		new Client
		{
			ClientId = "employee_app",
			AllowedGrantTypes = GrantTypes.Code,
			RequireClientSecret = false,
			RequirePkce = true,
			RedirectUris = { "http://localhost:5174/signin-callback" },
			PostLogoutRedirectUris = { "http://localhost:5174/" },
			AllowedCorsOrigins = { "http://localhost:5174" },
			AllowedScopes = { "openid", "profile", "api1" }
		}
	})
	.AddInMemoryIdentityResources(new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    })
    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://51.250.46.120:5003";
        options.MetadataAddress = "http://51.250.46.120:5003/.well-known/openid-configuration";
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


builder.Services.ConfigureApplicationCookie(opts =>
{
	opts.Cookie.SameSite = SameSiteMode.Lax;
	opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
});
builder.Services.Configure<CookieAuthenticationOptions>(
	IdentityServerConstants.DefaultCookieAuthenticationScheme,
	opts =>
	{
		opts.Cookie.SameSite = SameSiteMode.Lax;
		opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
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
app.UseRouting();
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorizationMiddleware();
app.UseAuthorization();

app.MapDefaultControllerRoute();


app.Run();
