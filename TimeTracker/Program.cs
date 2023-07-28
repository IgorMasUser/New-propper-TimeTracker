using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Extensions;
using TimeTracker.GraphQL;
using TimeTracker.Options;
using TimeTracker.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddHostedService<RefreshTokenCleanupService>();
builder.Services.AddScoped<IUserRepo, DBUserRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("DockerRedisConnection")));
builder.Services.AddGraphQLServer().AddQueryType<UserQuery>().AddMutationType<UserMutation>().AddFiltering().AddSorting();

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
var option = builder.Configuration.GetSection("JWT").Get<JWTOptions>();

IConfigurationBuilder configBuilder;

if (builder.Environment.IsDevelopment())
{
    configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false, true);
}
else
{
    configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
}

IConfigurationRoot configuration = configBuilder.Build();

builder.Services.AddAuthenticationServices(configuration);
builder.Services.AddMassTransitServices(configuration);
builder.Services.AddAuthorizationServices();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authorization}/{action=Authorize}/{id?}");

app.Run();
