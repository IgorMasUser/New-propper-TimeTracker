using MediatR;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public class UserQuery
    {
        [UseFiltering]
        [UseSorting]
        public async Task<IQueryable<User>> GetUsersAsync([Service] IMediator mediator)
        {
            var query = new GetUsersQuery();
            var users = await mediator.Send(query);
            return users.AsQueryable();
        }
    }
}
