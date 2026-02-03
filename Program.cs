using Microsoft.EntityFrameworkCore;
using CMC_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Services konfigurieren
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "CMC Bettenverwaltung API",
        Version = "v1",
        Description = "REST API für Krankenhaus-Bettenverwaltung"
    });
});

// SQLite Datenbank
builder.Services.AddDbContext<BettenDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS für WPF-App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWPF", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Datenbank erstellen & migrieren
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BettenDbContext>();
    db.Database.EnsureCreated();
}

// HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMC API v1");
        c.RoutePrefix = string.Empty; // ? WICHTIG!
    });
}


app.UseCors("AllowWPF");
app.UseAuthorization();
app.MapControllers();

app.Run();
