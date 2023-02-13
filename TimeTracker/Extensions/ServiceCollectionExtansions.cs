using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace TimeTracker.Extensions
{
    public static class ServiceCollectionExtansions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
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
                                    .GetBytes(configuration.GetSection("JWT:Key").Value)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration.GetSection("JWT:Issuer").Value,
                    ValidAudience = configuration.GetSection("JWT:Audience").Value
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

            return services;
        }    
    }
}


