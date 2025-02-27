using System.Security.Claims;
using System.Text;
using BusinessLogicLayer.Implementations;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Implementations;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Connecion String
var conn = builder.Configuration.GetConnectionString("PizzashopDB");
builder.Services.AddDbContext<PizzaShopDbContext>(q => q.UseNpgsql(conn));

// Helper
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<EncryptionService>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IGetUserRecordsRepository, UserRecordsRepository>();
builder.Services.AddScoped<ICountryDetailRepository, CountryDetailRepository>();
builder.Services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();



// Business Logic layer Services
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IResetPasswordService, ResetPasswordService>();
builder.Services.AddScoped<IUserDetailService, UserDetailService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IChangePasswordService, ChangePasswordService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();


// Add session Servies
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// JWt Token Configurations
builder.Services.AddAuthentication(x=>{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],  // The issuer of the token (e.g., your app's URL)
            ValidAudience = builder.Configuration["JwtConfig:Audience"], // The audience for the token (e.g., your API)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]?? "")), // The key to validate the JWT's signature
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name 
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check for the token in cookies
                var token = context.Request.Cookies["SuperSecretAuthToken"]; // Change "AuthToken" to your cookie name if it's different
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Redirect to login page when unauthorized instead of returning 401
                context.HandleResponse();
                context.Response.Redirect("/Home/Login");
                return Task.CompletedTask;
            }
        };
    }
);

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession(); //Enable Session

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
