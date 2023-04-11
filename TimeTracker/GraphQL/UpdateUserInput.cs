
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public record UpdateUserInput(int UserId, string Name, string Surname, string Email, int Role, float Salary,
    string ApprovalStatus, DateTime Date);
}
