namespace BackgroundService.Host.Extensions
{
    public interface MessageConsumed
    {
        DateTime Timestamp { get; set; }
        string Message { get; set; }
    }
}