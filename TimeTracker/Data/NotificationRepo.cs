using System.Text.Json;
using StackExchange.Redis;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly IConnectionMultiplexer redis;

        public NotificationRepo(IConnectionMultiplexer redis)
        {
            this.redis = redis;
        }

        public IEnumerable<NotificationMessage?>? GetAllNotifications()
        {
            var db = redis.GetDatabase();

            var completeSet = db.HashGetAll("message");

            if (completeSet.Length > 0)
            {
                var obj = Array.ConvertAll(completeSet, val =>
                    JsonSerializer.Deserialize<NotificationMessage>(val.Value)).ToList();
                return obj;
            }

            return null;
        }
    }
}
