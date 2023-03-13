namespace Notification.Host
{
    public record SendNotification
    {
        public string EmailAddress { get; init; }
        public string Body { get; init; }
    }

}
