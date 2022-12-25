using Microsoft.AspNetCore.Mvc;
using TimeTracker.DTOs;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    //Repository pattern
    public interface IUserRepo
    {
        IEnumerable<User> GetAttendanceOfUser(string search);
        Task CreateUser(User user);
        Task<User> EditAttendanceOfUser(int? id);
        Task EditAttendanceOfUser(User user);
        Task<User> GetUserToDelete(int? id);
        Task DeleteUser(int id);
        Task<User> GetDetailsOfUser(int? id);
        HashSet<User> GetAllEmployeesInfo();
    }
}
