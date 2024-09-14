using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeOptimizacionAPI.Data;
using SistemaDeOptimizacionAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Agrega la cadena de conexión a SQL Server en appsettings.json o aquí directamente
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura el DbContext para usar SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configura Identity con las opciones personalizadas para las contraseñas
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // No requiere un dígito
    options.Password.RequireLowercase = false; // No requiere una minúscula
    options.Password.RequireNonAlphanumeric = false; // No requiere un carácter no alfanumérico
    options.Password.RequireUppercase = false; // No requiere una mayúscula
    options.Password.RequiredLength = 6; // Requiere al menos 6 caracteres
    options.Password.RequiredUniqueChars = 1; // Requiere al menos 1 carácter único
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configura la autenticación JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configura los controladores y manejo de referencias circulares
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configura la tubería de middleware de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Aplica la política de CORS antes de la autenticación y autorización
app.UseCors("AllowAllOrigins");

app.UseAuthentication();  // Añadir autenticación
app.UseAuthorization();   // Añadir autorización

app.MapControllers();

app.Run();
