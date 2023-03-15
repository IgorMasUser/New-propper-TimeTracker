namespace Notification.Host.Extansions
{
    public interface MessageConsumed
    {
        DateTime Timestamp { get; set; }
        string Message { get; set; }
    }
}