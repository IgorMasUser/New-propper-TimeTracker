using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Extensions;
using TimeTracker.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddScoped<IUserRepo, DBUserRepo>();
builder.Services.AddTransient<ITokenService, TokenService>();

IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfigurationRoot configuration = configBuilder.Build();

builder.Services.AddAuthenticationServices(configuration);

builder.Services.AddMassTransitServices(configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireRole",
         policy => policy.RequireRole("1"));
    options.AddPolicy("Developer", policy => policy.RequireRole("3"));
    options.AddPolicy("TeamAccess", policy => policy.RequireRole("1","2"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authorization}/{action=Authorize}/{id?}");

app.Run();
