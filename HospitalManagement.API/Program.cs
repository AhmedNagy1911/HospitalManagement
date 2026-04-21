using HospitalManagement.API.Middleware;
using HospitalManagement.Domain.Constants;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// ?? Database ??????????????????????????????????????????????????
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ?? ASP.NET Identity ??????????????????????????????????????????
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password rules
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ?? JWT Authentication ????????????????????????????????????????
var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero   // ?? ??? tolerance ??? ??? expiry
    };
});

// ?? Authorization Policies ????????????????????????????????????
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        p => p.RequireRole(AppRoles.Admin));

    options.AddPolicy("MedicalStaff",
        p => p.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Technician));

    options.AddPolicy("FrontDesk",
        p => p.RequireRole(AppRoles.Admin, AppRoles.Receptionist));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen();
// ?? Swagger with JWT support ??????????????????????????????????

// Dependency Injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

// ?? Seed Roles + Default Admin ????????????????????????????????
await SeedAsync(app);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


static async Task SeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed Roles
    foreach (var role in AppRoles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed Default Admin
    const string adminEmail = "admin@hospital.com";
    const string adminPassword = "Admin@12345";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser is null)
    {
        adminUser = new ApplicationUser
        {
            FirstName = "Super",
            LastName = "Admin",
            Email = adminEmail,
            UserName = adminEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
    }
}
