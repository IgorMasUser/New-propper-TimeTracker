using Microsoft.AspNetCore.Mvc;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    //Repository pattern
    public interface IUserRepo
    {
        IQueryable<User> GetAttendanceOfUser(string search);
        Task<User> CreateUser(User user);
        Task<User> GetUserToEditById(int? id);
        Task<User> EditUserByName(User user);
        Task<User> GetDetailsOfUser(int? id);
        Task<User> FindUserToDelete(int? id);
        Task DeleteUser(int? id);
        HashSet<User> GetAllEmployeesInfo();
    }
}
