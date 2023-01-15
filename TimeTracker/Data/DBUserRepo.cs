using AutoMapper;
using System.Data.SqlTypes;
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
            if (requestedUser.StartedWorkDayAt == DateTime.MinValue || requestedUser.Date == DateTime.MinValue || requestedUser.FinishedWorkDayAt == DateTime.MinValue)
            {
                createdUser.StartedWorkDayAt = (DateTime)SqlDateTime.MinValue;
                createdUser.Date = (DateTime)SqlDateTime.MinValue;
                createdUser.FinishedWorkDayAt = (DateTime)SqlDateTime.MinValue;
                createdUser.TotalWorkedPerDay = (DateTime)SqlDateTime.MinValue;
                createdUser.UserWorkedPerRequestedPeriod = (DateTime)SqlDateTime.MinValue;
            }
            else
            {
                createdUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkedTimePerDay(ref createdUser);
                createdUser.Date = createdUser.StartedWorkDayAt;
            }

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
            mappedUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkedTimePerDay(ref mappedUser);
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
            var listOfUsers = db.User.Select(x => x).Where(y=>y.Date != (DateTime)SqlDateTime.MinValue);
            //foreach(var user in listOfUsers)
            //{
            //    if(user.Date == (DateTime)SqlDateTime.MinValue)
            //    {
            //       user.StartedWorkDayAt = DateTime.MinValue;
            //       user.Date = DateTime.MinValue;
            //       user.FinishedWorkDayAt = DateTime.MinValue;
            //       user.TotalWorkedPerDay = DateTime.MinValue;
            //       user.UserWorkedPerRequestedPeriod = DateTime.MinValue;
            //    }
            //}
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

        public async Task AuthorizeUser(User user, UserCreateDTO requestedUser)
        {
           
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
