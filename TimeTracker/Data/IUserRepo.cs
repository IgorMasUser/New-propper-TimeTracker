using Contracts;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.DTOs;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    //Repository pattern
    public interface IUserRepo
    {
        IEnumerable<User> GetAttendanceOfUser(string search);
        Task CreateUser(User user, UserCreateDTO requestedUser);
        Task<User> EditAttendanceOfUser(int? id);
        Task EditAttendanceOfUser(User user);
        Task<User> GetUserToDelete(int? id);
        Task DeleteUser(int id);
        Task<User> GetDetailsOfUser(int? id);
        HashSet<User> GetAllEmployeesInfo();
        bool CheckIfUserExists(User user, UserCreateDTO requestedUser);
        User GetUserDetails(User user);
        RefreshTokenProvider GetUserTokenDetails(string userName);
        Task<RefreshTokenProvider> SaveRefreshToken(int userId, string refreshToken);
        Task UpdateApprovalStatus(NewComerApprovalRequest userDetails);
        IEnumerable<User> NewComersRequestedForApproval();
        void GetNewComerApprovalStatus(Guid Id);
    }
}
