using MediatR;
using System.Collections.Generic;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public record GetUsersQuery : IRequest<List<User>>;
}
