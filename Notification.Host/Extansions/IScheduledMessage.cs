namespace Notification.Host.Extansions
{
    public interface IScheduledMessage
    {
        DateTime Timestamp { get; set; }
        public string scheduledMessage { get; set; }
    }
}