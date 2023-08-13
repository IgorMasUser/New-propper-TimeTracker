using TimeTracker.Data;

public class RefreshTokenCleanupService : IHostedService, IDisposable
{
    private readonly IServiceProvider services;
    private readonly ILogger<RefreshTokenCleanupService> logger;
    private Timer timer;

    public RefreshTokenCleanupService(IServiceProvider services, ILogger<RefreshTokenCleanupService> logger)
    {
        this.services = services;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Cleanup service started");

        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var now = DateTime.UtcNow;
            var expiredTokens = dbContext.RefreshTokenProvider
                .Where(t => t.RefreshTokenExpiresAt <= now)
                .ToList();
            logger.LogInformation("Refresh tokens checked for expiration");

            if (expiredTokens.Any())
            {
                dbContext.RefreshTokenProvider.RemoveRange(expiredTokens);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Refresh tokens cleaned");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during refresh token cleanup.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Cleanup service stopping");
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
