using System.Data.SqlTypes;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    //[ExtendObjectType(OperationTypeNames.Mutation)]
    public class UserMutation
    {
        //[UseDbContext(typeof(ApplicationDbContext))]
        public async Task<AddUserPayload> AddUser([GraphQLNonNullType] AddUserInput input,[Service] ApplicationDbContext context)
        {
            var user = new User
            {
                Name = input.Name,
                Surname = input.Surname,
                UserId = input.UserId,
                ApprovalStatus = input.ApprovalStatus,
                Date=input.Date,
                Email=input.Email,
                Role=input.Role,
                Salary = input.Salary  
            };

            context.Add(user);

            await context.SaveChangesAsync();

            return new AddUserPayload(user);
        }
    }
}
