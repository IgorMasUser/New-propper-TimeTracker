using Contracts;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using TimeTracker.BusinessLogic;
using TimeTracker.DTOs;
using TimeTracker.Models;
using StackExchange.Redis;
using System.Linq;

namespace TimeTracker.Data
{
    public class DBUserRepo : IUserRepo
    {
        private readonly ApplicationDbContext db;

        public DBUserRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddWorkedHours(User user, int userId)
        {
            var foundExistingUser = db.User.FirstOrDefault(x => x.UserId.Equals(userId));
            if(foundExistingUser != null)
            {
                db.User.Update(foundExistingUser);
                foundExistingUser.StartedWorkDayAt = user.StartedWorkDayAt;
                foundExistingUser.FinishedWorkDayAt = user.FinishedWorkDayAt;
                foundExistingUser.Date= DateTime.Today;
                foundExistingUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkedTimePerDay(user);

                await db.SaveChangesAsync();
            }
            else await Task.CompletedTask;  
        }

        public async Task CreateUser(User createdUser, UserCreateDTO requestedUser)
        {
            if (!db.User.Any(x => x.Email.Contains(requestedUser.Email)) || !db.User.Any(x => x.UserId.Equals(requestedUser.UserId)))
            {
                using (var hmac = new HMACSHA512())
                {
                    createdUser.PasswordSalt = hmac.Key;
                    createdUser.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestedUser.Password));
                }
                createdUser.StartedWorkDayAt = (DateTime)SqlDateTime.MinValue;
                createdUser.Date = (DateTime)SqlDateTime.MinValue;
                createdUser.FinishedWorkDayAt = (DateTime)SqlDateTime.MinValue;
                createdUser.TotalWorkedPerDay = (DateTime)SqlDateTime.MinValue;
                createdUser.UserWorkedPerRequestedPeriod = (DateTime)SqlDateTime.MinValue;
                createdUser.ApprovalStatus = "RequestedForApproval";
                db.User.Add(createdUser);
            }
            else
            {
                var foundExistingUser = db.User.FirstOrDefault(x => x.UserId.Equals(requestedUser.UserId));
                db.User.Update(foundExistingUser);
                foundExistingUser.Name = requestedUser.Name;
                foundExistingUser.UserId = requestedUser.UserId;
                foundExistingUser.Role = requestedUser.Role;
                foundExistingUser.Email = requestedUser.Email;
                foundExistingUser.Salary = requestedUser.Salary;
                foundExistingUser.Surname = requestedUser.Surname;
            }
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
            mappedUser.TotalWorkedPerDay = TimeCalculator.ToCalcWorkedTimePerDay(mappedUser);
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
            var getAllEmployees = db.User.Select(x => x).Where(d => d.ApprovalStatus.Contains("RequestApproved")).ToList();
            var setOfUsers = new UserTimeCalculator().GetTotalWorkedTimeForAllUsers(getAllEmployees);
            return setOfUsers;
        }

        public IEnumerable<User> GetAttendanceOfUser(string search)
        {
            //var listOfUsers = db.User.Select(x => x).Where(y => y.Date != (DateTime)SqlDateTime.MinValue); //if we want to see only users with filled attendance
            var listOfUsers = db.User.Select(x => x).Where(d => d.ApprovalStatus.Contains("RequestApproved"));

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

        public bool CheckIfUserExists(User user, UserCreateDTO requestedUser)
        {
            if (db.User.Any(x => x.Email.Contains(requestedUser.Email)))
            {
                var foundUser = db.User.FirstOrDefault(x => x.Email.Contains(requestedUser.Email));

                if (VerifyPassword(requestedUser.Password, foundUser.PasswordHash, foundUser.PasswordSalt))
                {
                    return true;
                }
            }
            return false;
        }

        public User GetUserDetails(User requestedUser)
        {
           var foundUser = db.User.FirstOrDefault(x => x.Email.Contains(requestedUser.Email));

            return foundUser;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<RefreshTokenProvider> SaveRefreshToken(int userId, string refreshToken)
        {
            RefreshTokenProvider tokenProvider = new RefreshTokenProvider();
            if (!db.RefreshTokenProvider.Any(x => x.UserId.Equals(userId)))
            {
                tokenProvider.RefreshTokenExpiresAt = DateTime.Now.AddDays(7);
                tokenProvider.RefreshTokenCreatedAt = DateTime.Now;
                tokenProvider.UserId = userId;
                tokenProvider.RefreshToken = refreshToken;
                db.RefreshTokenProvider.Add(tokenProvider);
                await db.SaveChangesAsync();
                return tokenProvider;
            }
            else
            {
                var foundExistingToken = db.RefreshTokenProvider.FirstOrDefault(x => x.UserId.Equals(userId));
                return foundExistingToken;
            }
        }

        public RefreshTokenProvider GetUserTokenDetails(string userName)
        {
            var obtainedUser = db.User.Where(p => p.Name.Contains(userName));
            //var tokenDetails = db.RefreshTokenProvider.FirstOrDefault(x=>x.RefreshToken.Equals(cookiesToken));
            var tokenDetails = db.RefreshTokenProvider.Where(p => obtainedUser.Any(p2 => p2.UserId == p.UserId)).FirstOrDefault();

            if (tokenDetails != null)
            {
                return tokenDetails;
            }
            return null;
        }

        public async Task UpdateApprovalStatus(NewComerApprovalRequest userDetails)
        {
            var foundUser = db.User.FirstOrDefault(x => x.ApprovalId.Equals(userDetails.ApprovalId));
            if (foundUser != null)
            {
                db.User.Update(foundUser);
                foundUser.ApprovalStatus = userDetails.State;
            }
            await db.SaveChangesAsync();
        }

        public IEnumerable<dynamic> NewComersRequestedForApproval()
        {
            var allRequestedForApprovalNewComers = db.User.Where(s => s.ApprovalStatus.Contains("RequestedForApproval"));
            var newComersWithRoleDetails = allRequestedForApprovalNewComers.Join(db.Roles, u => u.Role, r => r.UserRoleId, (u, r) => new
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Surname = u.Surname,
                Salary = u.Salary,
                SlaryLimit = r.SalaryLimit,
                RoleName = r.RoleName,
                ApprovalId = u.ApprovalId,
            });

            return newComersWithRoleDetails;
        }

        public IEnumerable<User> GetNewComersApprovalStatus()
        {
               var getNewComersApprovalStatus = db.User.Select(x => x);

                return getNewComersApprovalStatus;                    
        }
    }
}
