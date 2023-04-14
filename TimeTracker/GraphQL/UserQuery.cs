using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public class UserQuery
    {
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsers([Service] ApplicationDbContext context) //works
        {
            return context.User;
        }
    }
}
