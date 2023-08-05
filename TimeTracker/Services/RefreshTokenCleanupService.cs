using TimeTracker.Data;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly ILogger<RefreshTokenCleanupService> logger;

    public RefreshTokenCleanupService(IServiceProvider services, ILogger<RefreshTokenCleanupService> logger)
    {
        this.services = services;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Remove expired refresh tokens from the database
            await dbContext.RemoveExpiredRefreshTokensAsync();

            logger.Log(LogLevel.Information, "Refresh tokens cleaned");

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Delay for 5 minutes
        }
    }
}
