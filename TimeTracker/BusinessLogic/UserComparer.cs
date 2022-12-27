using System.Diagnostics.CodeAnalysis;
using TimeTracker.Models;

namespace TimeTracker.BusinessLogic
{
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User? x, User? y)
        {
            return String.Equals(x.UserId, y.UserId);
        }

        public int GetHashCode([DisallowNull] User obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}
