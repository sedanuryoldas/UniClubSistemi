using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVÝSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Yapýlandýrmasý (Kafa karýþýklýðýný önlemek için her þeyi tam adýyla yazdýk)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Etkinlik API", Version = "v1" });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Sadece Token yapýþtýrýn",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// Veritabaný
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Ayarlarý
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bu_cok_gizli_ve_uzun_bir_anahtar_cumlesi_olmalidir_12345!")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// --- 2. KANAL ---
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1"));

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();