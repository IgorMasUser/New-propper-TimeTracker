using MediatR;
using TimeTracker.Data;
using TimeTracker.GraphQL;
using TimeTracker.Models;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, User>
{
    private readonly ApplicationDbContext context;

    public AddUserCommandHandler(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<User> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            UserId = request.UserId,
            ApprovalStatus = request.ApprovalStatus,
            ApprovalId = Guid.NewGuid(),
            Date = request.Date,
            Email = request.Email,
            Role = request.Role,
            Salary = request.Salary
        };

        context.User.Add(user);
        await context.SaveChangesAsync();

        return user;
    }
}