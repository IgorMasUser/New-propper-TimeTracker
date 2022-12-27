using TimeTracker.Models;

namespace TimeTracker.BusinessLogic
{
    //Template Method Pattern
    public class UserTimeCalculator
    {
        public HashSet<User> GetTotalWorkedTimeForAllUsers(List<User> allselectedusers)
        {
            var setOfUsers = new HashSet<User>(new UserComparer());
            var userCalculatedTime = new List<DateTime>(); 

            AddUsersToHashSet(setOfUsers, allselectedusers);
            ChooseUniqueUsers(allselectedusers, userCalculatedTime);
            AsignRowNumberForUser(setOfUsers);

            return setOfUsers;
        }

        private void AddUsersToHashSet(HashSet<User> setOfUsers, List<User> allselectedusers)
        {
            foreach (var user in allselectedusers)
            {
                setOfUsers.Add(user);
            }
        }

        private void ChooseUniqueUsers(List<User> allselectedusers, List<DateTime> userCalculatedTime)
        {
            foreach (var item in allselectedusers)
            {
                foreach (var item2 in allselectedusers)
                {
                    if (item.UserId.Equals(item2.UserId))
                    {
                        item.UserWorkedPerRequestedPeriod = item.UserWorkedPerRequestedPeriod.Add(item2.TotalWorkedPerDay.TimeOfDay);
                    }
                }
                userCalculatedTime.Add(item.UserWorkedPerRequestedPeriod); //?
            }
        }

        private void AsignRowNumberForUser(HashSet<User> setOfUsers)
        {
            int i = 0;
            foreach (var count in setOfUsers)
            {
                i++;
                count.Numeration = i;
            }
        }
    }
}
