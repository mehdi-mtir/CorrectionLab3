using Catalog.API.Data;
using Catalog.API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Catalog API", Version = "v1" });
});

// DbContext
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Connection: {connectionString}");
    
    options.UseMySql(
        connectionString, 
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    );
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

var app = builder.Build();

// Migration avec retry - VERSION SIMPLIFIÉE
for (int i = 0; i < 10; i++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        
        Console.WriteLine($"[{i + 1}/10] Tentative de connexion à MySQL...");
        
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✓ Connexion réussie !");
            db.Database.Migrate();
            Console.WriteLine($"✓ Base initialisée avec {db.Products.Count()} produits");
            break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Erreur: {ex.Message}");
        if (i < 9)
        {
            Console.WriteLine("  Nouvelle tentative dans 5 secondes...");
            Thread.Sleep(5000);
        }
        else
        {
            Console.WriteLine("⚠ Impossible de se connecter à MySQL. L'API démarre quand même.");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (CatalogDbContext db) =>
{
    try
    {
        var connected = await db.Database.CanConnectAsync();
        if (connected)
        {
            var count = await db.Products.CountAsync();
            return Results.Ok(new { status = "healthy", products = count });
        }
        return Results.Problem("Cannot connect to database");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

Console.WriteLine("🚀 Catalog API démarrée sur le port 8080");
app.Run();

public partial class Program { }