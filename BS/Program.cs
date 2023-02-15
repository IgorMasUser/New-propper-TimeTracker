using System;
using System.Threading.Tasks;
using BackgroundService.Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<MyHostedService>();
var app = builder.Build();
app.Run();

