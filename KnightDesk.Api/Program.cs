using KnightDesk.Core.Application.Services;
using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using KnightDesk.Infrastructure.Repositories;
using KnightDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Render.com deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Enhanced Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KnightDesk API",
        Version = "v1",
        Description = "A comprehensive API for managing game accounts, users, and server information for KnightDesk application.",
        Contact = new OpenApiContact
        {
            Name = "KnightDesk Team",
            Email = "support@knightdesk.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add CORS for WPF client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWPF", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database Configuration
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Database connection string not found");

// Parse Render.com DATABASE_URL format if it exists
if (connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IServerInfoRepository, ServerInfoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Core Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IServerInfoService, ServerInfoService>();

// Infrastructure Services
builder.Services.AddScoped<DataSeedingService>();

//AutoMapper Configuration - Scan Core assembly for all Profile classes
builder.Services.AddAutoMapper(typeof(KnightDesk.Core.Application.Mappers.UserProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for easier API monitoring
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KnightDesk API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "KnightDesk API Documentation";
});

app.UseCors("AllowWPF");
app.UseHttpsRedirection();

// Map Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    
    var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
    await seedingService.SeedAllAsync();
}

app.Run();
