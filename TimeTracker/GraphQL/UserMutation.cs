using MediatR;
using TimeTracker.GraphQL;

public class UserMutation
{
    private readonly IMediator mediator;

    public UserMutation(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<AddUserPayload> AddUser(AddUserCommand input)
    {
        var newUser = await mediator.Send(input);
        return new AddUserPayload(newUser);
    }

    public async Task<AddUserPayload> UpdateUser(UpdateUserCommand input)
    {
        var updatedUser = await mediator.Send(input);
        return new AddUserPayload(updatedUser);
    }

    public async Task<int> DeleteUser(DeleteUserCommand input)
    {
        return await mediator.Send(input);
    }
}
