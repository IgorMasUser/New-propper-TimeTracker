namespace TimeTracker.GraphQL
{
    public record AddUserInput(int UserId,string Name, string Surname, string Email, int Role, float Salary,
        string ApprovalStatus, DateTime Date);
}