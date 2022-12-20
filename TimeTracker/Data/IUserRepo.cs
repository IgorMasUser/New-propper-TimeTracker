using Microsoft.AspNetCore.Mvc;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public interface IUserRepo
    {
        IActionResult GetAttendance(string search);
        public IActionResult CreateRole();
        public Task<IActionResult> CreateRole(User user);
        public Task<IActionResult> EditUserById(int id);
        public Task<IActionResult> EditUserByName(User user);
        public Task<IActionResult> GetDetails(int id);
        public Task<IActionResult> Delete(int id);
        public IActionResult GetAllEmployees();

    }
}
