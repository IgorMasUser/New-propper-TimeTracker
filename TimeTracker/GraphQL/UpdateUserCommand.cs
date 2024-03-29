﻿using MediatR;
using TimeTracker.Models;

namespace TimeTracker.GraphQL
{
    public record UpdateUserCommand(int UserId, string Name, string Surname, string Email, int Role, float Salary,
    string ApprovalStatus, DateTime Date) : IRequest<User>;
}
