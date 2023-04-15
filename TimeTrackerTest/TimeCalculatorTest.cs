using System;
using TimeTracker.Models;
using Xunit;
using TimeTracker.BusinessLogic;

namespace TimeTrackerTest
{
    public class TimeCalculatorTest
    {
        [Fact]
        public void Check_ToCalcWorkedTimePerDay()
        {
            // arrange
            try
            {
                User user = new User
                {
                    StartedWorkDayAt = DateTime.Parse("15/04/2023 09:05:00 AM"),
                    Break = 30,
                    FinishedWorkDayAt = DateTime.Parse("15/04/2023 05:15:00 PM")
                };

                // act                
                DateTime @break = new DateTime().AddMinutes(user.Break);
                TimeSpan temp = new DateTime().Subtract(@break);
                var totalInMinutes = user.FinishedWorkDayAt.Subtract(user.StartedWorkDayAt.Subtract(temp)).TotalMinutes; //
                double totalInMinutesManualyCalced = 460;

                var result = TimeCalculator.ToCalcWorkedTimePerDay(ref user);
                double resultInMinutes = result.Hour * 60 + result.Minute;

                // assert
                Assert.Equal(totalInMinutes, resultInMinutes);
                Assert.Equal(totalInMinutesManualyCalced, resultInMinutes);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert time");
            }


        }
    }
}