using TimeTracker.Models;

namespace TimeTracker.BusinessLogic
{
    public static class TimeCalculator
    {
        public static DateTime ToCalcWorkedTimePerDay(ref User user)
        {
            DateTime @break = new DateTime().AddMinutes(user.Break);
            TimeSpan temp = new DateTime().Subtract(@break);
            DateTime finished = user.FinishedWorkDayAt;
            DateTime started = user.StartedWorkDayAt;
            TimeSpan workDay = finished.Subtract(started.Subtract(temp));
            DateTime total = Convert.ToDateTime(workDay.ToString());

            return total;
        }
    }
}
