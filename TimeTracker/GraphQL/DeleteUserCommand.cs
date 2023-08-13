using MediatR;

namespace TimeTracker.GraphQL
{
    public record DeleteUserCommand(int UserId) : IRequest<int>;
}
