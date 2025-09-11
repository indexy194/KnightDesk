using KnightDesk.Core.Application.Services;
using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using KnightDesk.Infrastructure.Repositories;
using KnightDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Render.com deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
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

// Database Configuration with comprehensive logging
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

Console.WriteLine("=== DATABASE CONFIGURATION DEBUG ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"DATABASE_URL present: {!string.IsNullOrEmpty(dbUrl)}");

if (!string.IsNullOrEmpty(dbUrl))
{
    Console.WriteLine($"DATABASE_URL length: {dbUrl.Length}");
    Console.WriteLine($"DATABASE_URL starts with 'postgres://': {dbUrl.StartsWith("postgres://")}");
    
    // Log first and last 10 characters for debugging (without exposing full credentials)
    if (dbUrl.Length > 20)
    {
        Console.WriteLine($"DATABASE_URL format: {dbUrl.Substring(0, 10)}...{dbUrl.Substring(dbUrl.Length - 10)}");
    }
    
    try
    {
        connectionString = ConvertPostgresUrlToNpgsql(dbUrl);
        Console.WriteLine("✅ DATABASE_URL successfully converted to Npgsql format");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error converting DATABASE_URL: {ex.Message}");
        throw;
    }
}
else
{
    Console.WriteLine("⚠️ DATABASE_URL not found, using fallback connection");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("No database connection string available");
    Console.WriteLine("Using DefaultConnection from appsettings.json");
}

Console.WriteLine("=== END DATABASE CONFIGURATION DEBUG ===");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Helper to convert postgres:// to Npgsql format
static string ConvertPostgresUrlToNpgsql(string url)
{
    Console.WriteLine("Converting DATABASE_URL to Npgsql format...");
    
    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':');
    
    Console.WriteLine($"Parsed components - Host: {uri.Host}, Port: {uri.Port}, Database: {uri.AbsolutePath.TrimStart('/')}, Username: {userInfo[0]}");
    
    var result = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    
    Console.WriteLine("✅ Conversion completed");
    return result;
}

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

// Initialize database with comprehensive logging
Console.WriteLine("=== DATABASE INITIALIZATION DEBUG ===");
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Log the actual connection string being used (without password)
        var connectionString = context.Database.GetConnectionString();
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Mask password for logging
            var maskedConnectionString = connectionString.Contains("Password=") 
                ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]*", "Password=***")
                : connectionString;
            Console.WriteLine($"Using connection string: {maskedConnectionString}");
        }
        
        Console.WriteLine("Attempting to connect to database...");
        
        // Test database connection with timeout
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        Console.WriteLine("Testing database connectivity...");
        var canConnect = await context.Database.CanConnectAsync(cancellationTokenSource.Token);
        
        if (!canConnect)
        {
            Console.WriteLine("❌ CanConnectAsync returned false");
            Console.WriteLine("This usually means:");
            Console.WriteLine("1. Database server is not reachable");
            Console.WriteLine("2. Wrong host/port in connection string");
            Console.WriteLine("3. Database credentials are incorrect");
            Console.WriteLine("4. SSL/TLS configuration issues");
            Console.WriteLine("5. Database server is not accepting connections");
            
            throw new InvalidOperationException("Cannot connect to database - CanConnectAsync returned false");
        }
        
        Console.WriteLine("✅ Database connection successful!");
        
        // Ensure database is created
        Console.WriteLine("Ensuring database schema exists...");
        await context.Database.EnsureCreatedAsync(cancellationTokenSource.Token);
        Console.WriteLine("✅ Database schema ensured.");
        
        // Seed data
        Console.WriteLine("Starting database seeding...");
        var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
        await seedingService.SeedAllAsync();
        Console.WriteLine("✅ Database seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
        Console.WriteLine($"Exception type: {ex.GetType().Name}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().Name}");
        }
        
        Console.WriteLine("=== TROUBLESHOOTING TIPS ===");
        Console.WriteLine("1. Verify DATABASE_URL environment variable is set correctly");
        Console.WriteLine("2. Check that PostgreSQL database service is running on Render");
        Console.WriteLine("3. Ensure database credentials are valid");
        Console.WriteLine("4. Verify network connectivity to database host");
        Console.WriteLine("=== END TROUBLESHOOTING TIPS ===");
        
        Console.WriteLine($"Full stack trace: {ex.StackTrace}");
        throw;
    }
}
Console.WriteLine("=== END DATABASE INITIALIZATION DEBUG ===");

app.Run();
