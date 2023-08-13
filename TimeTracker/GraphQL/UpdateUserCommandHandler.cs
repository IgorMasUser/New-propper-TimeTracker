using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data;
using TimeTracker.GraphQL;
using TimeTracker.Models;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly ApplicationDbContext context;

    public UpdateUserCommandHandler(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.UserId == request.UserId);

        if (user != null)
        {
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.UserId = request.UserId;
            user.ApprovalStatus = request.ApprovalStatus;
            user.ApprovalId = Guid.NewGuid();
            user.Date = request.Date;
            user.Email = request.Email;
            user.Role = request.Role;
            user.Salary = request.Salary;
            await context.SaveChangesAsync();
        }

        return user;
    }
}
