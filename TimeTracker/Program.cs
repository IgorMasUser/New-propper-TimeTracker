using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Extensions;
using TimeTracker.GraphQL;
using TimeTracker.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddScoped<IUserRepo, DBUserRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("DockerRedisConnection")));
builder.Services.AddGraphQLServer().AddQueryType<UserQuery>().AddFiltering().AddSorting();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("JWT:Key").Value)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var dataToken = context.Request.Cookies["accessTokenForDataRetriving"];
            if (string.IsNullOrEmpty(dataToken))
            {
                context.Response.Redirect("/Authorization/Authorize");
            }
            else
            {
                context.Response.Redirect("/Authorization/Refresh");
            }
            context.HandleResponse();
            return Task.FromResult(0);
        }

    };
});

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

//builder.Services.AddAuthenticationServices(configuration);
builder.Services.AddMassTransitServices(configuration);
builder.Services.AddAuthorizationServices();

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
