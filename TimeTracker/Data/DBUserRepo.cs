using TimeTracker.BusinessLogic;
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

        public async Task<User> CreateUser(User user)
        {
            user.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref user);
            user.Date = user.StartedWorkDayAt;
            db.User.Add(user);
            await db.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUser(int? id)
        {
            var user = await db.User.FindAsync(id);
            if (user != null)
            {
                db.User.Remove(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task<User> EditUserByName(User user)
        {
            db.User.Update(user);
            user.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref user);
            user.Date = user.StartedWorkDayAt;
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<User> FindUserToDelete(int? id)
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

        public IQueryable<User> GetAttendanceOfUser(string search)
        {
            var data = db.User.Select(x => x);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(a => a.Name.Contains(search) || a.Surname.Contains(search));
            }

            return data;
        }

        public async Task<User> GetDetailsOfUser(int? id)
        {
            var user = await db.User.FindAsync(id);

            return user;
        }

        public async Task<User> GetUserToEditById(int? id)
        {
            var user = await db.User.FindAsync(id);
            return user;
        }

    }
}
