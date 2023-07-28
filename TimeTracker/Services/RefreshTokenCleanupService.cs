using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeTracker.Data;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider services;

    public RefreshTokenCleanupService(IServiceProvider services)
    {
        this.services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Remove expired refresh tokens from the database
            await dbContext.RemoveExpiredRefreshTokensAsync();

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Delay for 5 minutes
        }
    }
}
