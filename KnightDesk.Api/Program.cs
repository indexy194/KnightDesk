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

// Database Configuration
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"DATABASE_URL present: {!string.IsNullOrEmpty(databaseUrl)}");

if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine($"Raw DATABASE_URL length: {databaseUrl.Length}");
    Console.WriteLine($"DATABASE_URL starts with postgres://: {databaseUrl.StartsWith("postgres://")}");
    
    try
    {
        // Parse Render.com DATABASE_URL format (postgres://user:password@host:port/database)
        if (databaseUrl.StartsWith("postgres://"))
        {
            var uri = new Uri(databaseUrl);
            
            // Validate URI components
            if (string.IsNullOrEmpty(uri.Host))
                throw new ArgumentException("Database host is missing from DATABASE_URL");
            
            if (string.IsNullOrEmpty(uri.UserInfo))
                throw new ArgumentException("Database credentials are missing from DATABASE_URL");
            
            var userInfo = uri.UserInfo.Split(':');
            if (userInfo.Length != 2)
                throw new ArgumentException("Database credentials format is invalid in DATABASE_URL");
            
            var database = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrEmpty(database))
                throw new ArgumentException("Database name is missing from DATABASE_URL");
            
            connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            
            Console.WriteLine($"Parsed connection - Host: {uri.Host}, Port: {uri.Port}, Database: {database}, Username: {userInfo[0]}");
        }
        else
        {
            // Assume it's already in Npgsql format
            connectionString = databaseUrl;
            Console.WriteLine("Using DATABASE_URL as-is (not postgres:// format)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
        throw new InvalidOperationException($"Invalid DATABASE_URL format: {ex.Message}", ex);
    }
}
else
{
    // Fallback to local development connection
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Database connection string not found");
    Console.WriteLine("Using local development connection string");
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
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Attempting to connect to database...");
        
        // Test database connection with timeout
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var canConnect = await context.Database.CanConnectAsync(cancellationTokenSource.Token);
        
        if (!canConnect)
        {
            throw new InvalidOperationException("Cannot connect to database - CanConnectAsync returned false");
        }
        
        Console.WriteLine("✅ Database connection successful!");
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync(cancellationTokenSource.Token);
        Console.WriteLine("✅ Database schema ensured.");
        
        // Seed data
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
        }
        
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}

app.Run();
