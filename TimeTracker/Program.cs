using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddScoped<IUserRepo, DBUserRepo>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();

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

        //OnAuthenticationFailed = context =>
        //{

        //    context.Response.Redirect("/Authorization/Authorize");
        //    return Task.FromResult(0);

        //},

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


//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddCookie(x =>
//{
//    x.Cookie.Name = "accessToken";

//}).AddJwtBearer(x =>
//{
//    x.RequireHttpsMetadata = false;
//    x.SaveToken = true;
//    x.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
//                        .GetBytes(builder.Configuration.GetSection("JWT:Key").Value)),
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
//        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value
//    };
//    x.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            context.Token = context.Request.Cookies["accessToken"];
//            return Task.CompletedTask;
//        }
//    };
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseStatusCodePages(async context =>
//{
//    var response = context.HttpContext.Response;

//    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
//    {
//        response.Redirect("/Authorization/Authorize");
//    }
//});


app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authorization}/{action=Authorize}/{id?}");

app.Run();
