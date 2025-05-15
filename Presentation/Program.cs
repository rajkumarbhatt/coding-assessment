using System.Text;
using BLL.Interfaces;
using BLL.Services;
using DAL.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .WriteTo.File("logs/log.txt")
        .Filter.ByIncludingOnly(logEvent =>
            logEvent.Properties["SourceContext"].ToString().Contains("BLL.Services.JwtService") ||
            logEvent.Properties["SourceContext"].ToString().Contains("BLL.Services.AdminService") ||
            logEvent.Properties["SourceContext"].ToString().Contains("BLL.Services.UserService") ||
            logEvent.Properties["SourceContext"].ToString().Contains("BLL.Services.LoginService"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5170",
            ValidAudience = "http://localhost:5170",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test1232133454353533636gfhgfhxfdsfsdfsdfghgfhfghfghgfhfghfhfgh"))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.MapControllerRoute(
    name: "error",
    pattern: "{*url}",
    defaults: new { controller = "Home", action = "PageNotFound" }
);

app.Run();