using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MySqlConnector;
using MySql.EntityFrameworkCore;

using System;
using System.Text;

using StarWarsTcgApi.Infrastructure;
using StarWarsTcgApi.Application;
using StarWarsTcg.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Uncomment the line below if you want to use Razor Pages or MVC views
//  builder.Services.AddRazorPages();
// Uncomment the line below if you want to use MVC with views
//  builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
// Uncomment the line below if you want to use MVC with views and controllers   
//  builder.Services.AddControllersWithViews();

//configure Entity Framework Core with MySQL
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new InvalidOperationException("Connection string 'IdentityConnection' is not configured.");
}
builder.Services.AddMySqlDataSource(identityConnectionString);
builder.Services.AddDbContext<StarWarsTcg.Security.IdentityDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(identityConnectionString, ServerVersion.AutoDetect(identityConnectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

// Configure Authorization Policies (Optional, but good practice for role-based authorization)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequirePlayer", policy => policy.RequireRole("Player"));
    options.AddPolicy("RequireWatcher", policy => policy.RequireRole("Watcher"));
});

//Configure Identity
builder.Services.AddIdentity<StarWarsTcg.Security.User, StarWarsTcg.Security.Role>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 4;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<StarWarsTcg.Security.IdentityDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var durationInMinutes = Convert.ToDouble(jwtSettings["DurationInMinutes"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

//configure the application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(durationInMinutes);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddInfrastructure(builder.Configuration);

//Register the Application services
builder.Services.AddApplication();


var app = builder.Build();

// Configure the HTTP request pipeline.
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-9.0#configure-services-and-middleware-by-environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
else
{
    app.UseExceptionHandler(); // "/Error"
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication(); // Must occur before UseAuthorization
app.UseAuthorization();

//Configure the MVC middleware
//app.UseRouting();
//app.UseStaticFiles();
app.MapControllers(
    // name: "default", pattern: "{controller=Home}/{action=Index}/{id?}"    
);

app.Run();
