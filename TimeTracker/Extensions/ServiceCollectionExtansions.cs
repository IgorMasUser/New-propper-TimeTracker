using Contracts;
using MassTransit;
using MassTransitSchedulingTest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notification.Host;
using System.Text;

namespace TimeTracker.Extensions
{
    public static class ServiceCollectionExtansions
    {
        //public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(x =>
        //    {
        //        x.RequireHttpsMetadata = false;
        //        x.SaveToken = true;
        //        x.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        //                            .GetBytes(configuration.GetSection("JWT:Key").Value)),
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = configuration.GetSection("JWT:Issuer").Value,
        //            ValidAudience = configuration.GetSection("JWT:Audience").Value
        //        };
        //        x.Events = new JwtBearerEvents
        //        {
        //            OnMessageReceived = context =>
        //            {
        //                context.Token = context.Request.Cookies["accessToken"];
        //                return Task.CompletedTask;
        //            },
        //            OnChallenge = context =>
        //            {
        //                var dataToken = context.Request.Cookies["accessTokenForDataRetriving"];
        //                if (string.IsNullOrEmpty(dataToken))
        //                {
        //                    context.Response.Redirect("/Authorization/Authorize");
        //                }
        //                else
        //                {
        //                    context.Response.Redirect("/Authorization/Refresh");
        //                }
        //                context.HandleResponse();
        //                return Task.FromResult(0);
        //            }

        //        };
        //    });

        //    return services;
        //}

        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Developers",
                     policy => policy.RequireRole("1", "4"));
                options.AddPolicy("Manager", policy => policy.RequireRole("1", "3"));
                options.AddPolicy("TeamAccess", policy => policy.RequireRole("1", "3", "2"));
                options.AddPolicy("HR", policy => policy.RequireRole("1", "2"));
            });

            return services;
        }

        public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
        services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddConsumer<RequestConsumer>();
                cfg.AddConsumer<ScheduledNotificationConsumer>();
                cfg.UsingRabbitMq((cxt, cfg) =>
                 {
                     string hostName = configuration.GetSection("RabbitMQ:HostName").Value;
                     Console.WriteLine(hostName);

                     cfg.Host(hostName, "/", h =>
                     {
                         h.Username("guest");
                         h.Password("guest");
                     });

                     cfg.ConfigureEndpoints(cxt);
                 });

                cfg.AddRequestClient<ISimpleRequest>();
            });

            return services;
        }
    }
}


