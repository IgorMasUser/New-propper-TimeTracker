using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data;
using TimeTracker.GraphQL;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
{
    private readonly ApplicationDbContext context;

    public DeleteUserCommandHandler(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.UserId == request.UserId);

        if (user != null)
        {
            context.User.Remove(user);
            await context.SaveChangesAsync();
            return user.UserId;
        }

        return 0;
    }
}
