using AutoMapper;
using System.Security.Cryptography;
using TimeTracker.BusinessLogic;
using TimeTracker.DTOs;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public class DBUserRepo : IUserRepo
    {
        private readonly ApplicationDbContext db;

        public DBUserRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task CreateUser(User createdUser, UserCreateDTO requestedUser)
        {
            using (var hmac = new HMACSHA512())
            {
                createdUser.PasswordSalt = hmac.Key;
                createdUser.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestedUser.Password));
            }

            //createdUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref createdUser);
            //createdUser.Date = createdUser.StartedWorkDayAt;
            db.User.Add(createdUser);
            await db.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var result = await db.User.FindAsync(id);
            db.User.Remove(result);
            await db.SaveChangesAsync();
        }

        public async Task<User> GetUserToDelete(int? id)
        {
            var user = await db.User.FindAsync(id);
            return user;
        }

        public async Task EditAttendanceOfUser(User mappedUser)
        {
            db.User.Update(mappedUser);
            mappedUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref mappedUser);
            mappedUser.Date = mappedUser.StartedWorkDayAt;
            await db.SaveChangesAsync();
        }

        public async Task<User> EditAttendanceOfUser(int? id)
        {
            var user = await db.User.FindAsync(id);
            return user;
        }

        public HashSet<User> GetAllEmployeesInfo()
        {
            using (db)
            {
                var data = db.User.Select(x => x).ToList();
                var setOfUsers = new UserTimeCalculator().GetTotalWorkedTimeForAllUsers(data);
                return setOfUsers;
            }
        }

        public IEnumerable<User> GetAttendanceOfUser(string search)
        {
            var listOfUsers = db.User.Select(x => x);
            if (!string.IsNullOrEmpty(search))
            {
                listOfUsers = listOfUsers.Where(a => a.Name.Contains(search) || a.Surname.Contains(search));
            }
            return listOfUsers;
        }

        public async Task<User> GetDetailsOfUser(int? id)
        {
            var user = await db.User.FindAsync(id);
            return user;
        }

        //public async Task RegisterUser(User createdUser, UserCreateDTO requestedUser)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        createdUser.PasswordSalt = hmac.Key;
        //        createdUser.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestedUser.Password));
        //    }
        //    db.User.Add(createdUser);
        //    await db.SaveChangesAsync();
        //}
    }
}
