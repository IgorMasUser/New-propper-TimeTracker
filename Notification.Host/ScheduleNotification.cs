namespace Notification.Host
{
    public record ScheduleNotification
    {
        public DateTime DeliveryTime { get; init; }
        public string EmailAddress { get; init; }
        public string Body { get; init; }
    }

}
