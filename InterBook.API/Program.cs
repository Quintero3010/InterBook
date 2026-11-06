using InterBook.API.Data;
using InterBook.API.Interfaces;
using InterBook.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ============= CONFIGURACIÓN DE SERVICIOS =============

// Configurar Controllers con JSON case-insensitive
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignorar mayúsculas/minúsculas en JSON
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        // Convertir enums a strings
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Ignorar propiedades nulas
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<InterBookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios
builder.Services.AddScoped<IAuthService, AuthService>();
// Comenta estos si no existen aún:
builder.Services.AddScoped<IIntercambioService, IntercambioService>();
builder.Services.AddScoped<ILibroService, LibroService>();

// Configurar JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key no configurada");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Sin tolerancia de tiempo
    };
});

// Configurar CORS (importante para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Agregar servicios de MVC para vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ============= CONFIGURACIÓN DEL PIPELINE =============

//// Swagger solo en desarrollo
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Para wwwroot

app.UseCors("AllowAll"); // Habilitar CORS

app.UseRouting();

app.UseAuthentication(); // Debe ir ANTES de Authorization
app.UseAuthorization();

// Mapear controladores API y MVC
app.MapControllers(); // Para API
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Para vistas

app.Run();