// Owner: Reyes | Type: top-level program (composition root: DI, JWT, Swagger, migrations)
using System.Text;
using GymApi.Data;
using GymApi.Models;
using GymApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Database (SQL Server via EF Core) ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- JWT authentication ----
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });
builder.Services.AddAuthorization();

// ---- Our layers (interface -> implementation) ----
builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gym Membership & Class Booking API", Version = "v1" });

    // "Authorize" button in Swagger: paste the token from /auth/login
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the token (no 'Bearer ' prefix)."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---- Apply migrations + seed test accounts on startup (dev convenience, as in the lesson) ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();
    await DbSeeder.SeedAsync(db, hasher);
}

app.UseSwagger();
app.UseSwaggerUI();

// "/" -> wwwroot/login.html (the site always starts on the login page)
var defaultFiles = new DefaultFilesOptions();
defaultFiles.DefaultFileNames.Clear();
defaultFiles.DefaultFileNames.Add("login.html");
app.UseDefaultFiles(defaultFiles);
app.UseStaticFiles();    // serves the HTML/JS front end from wwwroot

app.UseAuthentication(); // 1) who are you?
app.UseAuthorization();  // 2) are you allowed?

app.MapControllers();
app.Run();
