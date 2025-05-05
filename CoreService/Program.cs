using CoreService.Models.Database;
using CoreService.Services;
using CoreService.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CoreService.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoreService.Integrations.Http.UserService;
using CoreService.Helpers.Cache;
using CoreService.Integrations.AMQP.RabbitMQ.Producer;
using CoreService.Integrations.AMQP.RabbitMQ.Consumer;
using CoreService.Integrations.Http.UniRate;
using CoreService.Helpers;
using CoreService.Middlewares.Failure;
using Polly;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RefreshOnIssuerKeyNotFound = true;
        options.RequireHttpsMetadata = false;
        options.Authority = "http://51.250.46.120:5003";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ARCHPAT Core Service Api",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services.AddDbContext<CoreDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISupportService, SupportService>();

builder.Services.AddSingleton<IUserServiceAdapter, UserServiceAdapter>();
builder.Services.AddSingleton<IUniRateAdapter, UniRateAdapter>();

builder.Services.AddSingleton<IUserParametersCache, UserParametersCache>();

builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();
builder.Services.AddHostedService<UserBansConsumer>();
builder.Services.AddHostedService<TransactionRequestConsumer>();

builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();

ConfigurationHelper.Initialize(builder.Configuration);

builder.Services.AddHttpClient("ResilientClient")
    .AddTransientHttpErrorPolicy(policy => policy
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            onRetry: (outcome, timespan, attempt, context) =>
            {
                Console.WriteLine($"Retry {attempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message}");
            }))
    .AddTransientHttpErrorPolicy(policy => policy
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(15),
            onBreak: (result, breakDelay) =>
            {
                Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds}s due to {result.Exception?.Message}");
            },
            onReset: () => Console.WriteLine("Circuit reset"),
            onHalfOpen: () => Console.WriteLine("Circuit half-open")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        Console.WriteLine("Applying migrations...");
        db.Database.Migrate();
        Console.WriteLine("Migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseMonitoringMiddlewareService();

app.UseExceptionMiddleware();

app.UseMiddleware<FailureImitatorMiddleware>();

app.UseAuthorizationMiddleware();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(30)
});

app.Run();
