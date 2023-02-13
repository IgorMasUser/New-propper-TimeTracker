namespace BackgroundService.Host.Extensions
{
    public interface IScheduledMessage
    {
        DateTime Timestamp { get; set; }
        public string scheduledMessage { get; set; }
    }
}