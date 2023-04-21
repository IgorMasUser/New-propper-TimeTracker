using System;
using TimeTracker.Models;
using Xunit;
using TimeTracker.BusinessLogic;
using System.Collections.Generic;
using System.Linq;

namespace TimeTrackerTest
{
    public class TimeCalculatorTest
    {
        [Fact]
        public void Check_ToCalcWorkedTimePerDay()
        {

            try
            {
                // arrange
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

                var result = TimeCalculator.ToCalcWorkedTimePerDay(user);
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

        [Fact]
        public void Check_TotalWorkedTimeForAllUsers()
        {
            try
            {
                // arrange
                List<User> listOfUsers = new List<User>
                {
                    new User {
                    UserId = 1111,
                    StartedWorkDayAt = DateTime.Parse("19/04/2023 09:05:00 AM"),
                    Break = 30,
                    FinishedWorkDayAt = DateTime.Parse("19/04/2023 05:15:00 PM")
                              },
                    new User {
                    UserId = 2222,
                    StartedWorkDayAt = DateTime.Parse("19/04/2023 09:00:00 AM"),
                    Break = 30,
                    FinishedWorkDayAt = DateTime.Parse("19/04/2023 05:10:00 PM")
                    },
                    new User {
                    UserId = 1111,
                    StartedWorkDayAt = DateTime.Parse("19/04/2023 09:05:00 AM"),
                    Break = 30,
                    FinishedWorkDayAt = DateTime.Parse("19/04/2023 05:15:00 PM"),
                    },
                };
                // act
                foreach (var user in listOfUsers)
                {
                    var totalWorkedPerDay = TimeCalculator.ToCalcWorkedTimePerDay(user);
                    user.TotalWorkedPerDay = totalWorkedPerDay;
                }

                var listOfUniqueUsers = new UserTimeCalculator().GetTotalWorkedTimeForAllUsers(listOfUsers);
                var chosenUser = listOfUniqueUsers.Where(x => x.UserId == 1111).FirstOrDefault();
                double expactedInMinutes = chosenUser.UserWorkedPerRequestedPeriod.Hour * 60 + chosenUser.UserWorkedPerRequestedPeriod.Minute;
                var manualyCalcedInMinutes = 920;

                // assert
                Assert.Equal(manualyCalcedInMinutes, expactedInMinutes);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert time");
            }
        }
    }
}