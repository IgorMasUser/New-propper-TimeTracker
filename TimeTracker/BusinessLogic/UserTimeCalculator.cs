using TimeTracker.Models;

namespace TimeTracker.BusinessLogic
{
    //Template Method Pattern
    public class UserTimeCalculator
    {
        public HashSet<User> GetTotalWorkedTimeForAllUsers(List<User> allselectedusers)
        {
            var setOfUsers = new HashSet<User>(new UserComparer());
            //var listOfUsers = new List<User>();
            var userCalculatedTime = new List<DateTime>(); //?

            ChooseUniqueUsers(allselectedusers, userCalculatedTime);
            AsignRowNumberForUser(setOfUsers);

            return setOfUsers;
        }

        private static void AsignRowNumberForUser(HashSet<User> setOfUsers)
        {
            foreach (var count in setOfUsers)
            {
                int i = 0;
                i++;
                count.Numeration = i;
            }
        }

        private static void ChooseUniqueUsers(List<User> allselectedusers, List<DateTime> userCalculatedTime)
        {
            foreach (var item in allselectedusers)
            {
                foreach (var item2 in allselectedusers)
                {
                    if (item.UserId.Equals(item2.UserId))
                    {
                        item.TotalWorkedPerDay = item.TotalWorkedPerDay.Add(item2.TotalWorkedPerDay.TimeOfDay);
                    }
                }
                userCalculatedTime.Add(item.TotalWorkedPerDay); //?
            }
        }
    }
}
