using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public class UserMutation
    {
        public async Task<AddUserPayload> AddUser([GraphQLNonNullType] AddUserInput input, [Service] ApplicationDbContext context)
        {
            var user = new User
            {
                Name = input.Name,
                Surname = input.Surname,
                UserId = input.UserId,
                ApprovalStatus = input.ApprovalStatus,
                Date = input.Date,
                Email = input.Email,
                Role = input.Role,
                Salary = input.Salary
            };

            using (context)
            {
                context.Add(user);
                await context.SaveChangesAsync();
            }
            return new AddUserPayload(user);
        }

        public async Task<AddUserPayload> UpdateUser([GraphQLNonNullType] UpdateUserInput input, int? userId, [Service] ApplicationDbContext context)
        {
            using (context)
            {
                var foundUser = context.User.Where(x => x.UserId == userId).FirstOrDefault();

                foundUser.Name = input.Name;
                foundUser.Surname = input.Surname;
                foundUser.UserId = input.UserId;
                foundUser.ApprovalStatus = input.ApprovalStatus;
                foundUser.Date = input.Date;
                foundUser.Email = input.Email;
                foundUser.Role = input.Role;
                foundUser.Salary = input.Salary;

                context.Update(foundUser);
                await context.SaveChangesAsync();

                return new AddUserPayload(foundUser);
            }
        }

        public async Task<int> DeleteUser([GraphQLNonNullType] int? userId, [Service] ApplicationDbContext context)
        {
            var userToDelete = context.User.Where(x => x.UserId == userId).FirstOrDefault();
            if (userToDelete != null)
            {
                context.User.Remove(userToDelete);
                await context.SaveChangesAsync();
                return userToDelete.UserId;
            }

            return 0;
        }
    }
}
