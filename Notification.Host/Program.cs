
using Notification.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<RemindingService>();
var app = builder.Build();
app.Run();

