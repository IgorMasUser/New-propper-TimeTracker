using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<User>>
    {
        private readonly ApplicationDbContext context;

        public GetUsersQueryHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await context.User.ToListAsync(cancellationToken);
            return users;
        }
    }
}
