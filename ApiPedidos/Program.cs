using ApiPedidos.Data;
using ApiPedidos.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core - SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? "Server=(localdb)\\MSSQLLocalDB;Database=PedidosDb;Trusted_Connection=True;TrustServerCertificate=True";
    options.UseSqlServer(cs);
});

// Controllers
builder.Services.AddControllers();

// CORS (permitir a ClienteApi por defecto)
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// JWT (opcional)
var jwtEnabled = builder.Configuration.GetValue("Jwt:Enabled", false);
if (jwtEnabled)
{
    var key = builder.Configuration["Jwt:Key"] ?? "DEV-KEY-CHANGE-ME";
    var issuer = builder.Configuration["Jwt:Issuer"] ?? "PedidosApi";
    var audience = builder.Configuration["Jwt:Audience"] ?? "PedidosConsumers";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });
}

var app = builder.Build();

// Migrar BD al iniciar (para demo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

if (jwtEnabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.Run();