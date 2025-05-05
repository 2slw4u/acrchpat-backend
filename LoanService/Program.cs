using System.Text;
using LoanService.Database;
using LoanService.Integrations;
using LoanService.Integrations.HttpRequesters;
using LoanService.Middleware;
using LoanService.Services;
using LoanService.Services.Interfaces;
using LoanService.Services.Logic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

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

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Loan API", Version = "1.0" });
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

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services.AddScoped<IRateService, RateService>();
builder.Services.AddScoped<ILoanManagerService, LoanManagerService>();
builder.Services.AddSingleton<IRabbitMqTransactionRequestProducer, RabbitMqTransactionRequestProducer>();
builder.Services.AddSingleton<IRabbitMqLogOperationResultProducer, RabbitMqLogOperationResultProducer>();
builder.Services.AddSingleton<UserRequester>();
builder.Services.AddSingleton<CoreRequester>();
builder.Services.AddHostedService<RabbitMqTransactionResultConsumer>();
builder.Services.AddHostedService<LoanAutopaymentProcessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

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
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("Applying migrations...");
        db.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

app.UseCors("AllowAll");

app.UseMiddleware<MonitoringMiddlewareService>();
app.UseExceptionMiddleware();
/*app.UseMiddleware<FailureImitatorMiddleware>();*/
app.UseMiddleware<AuthorizationMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
