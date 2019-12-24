using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Utils;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Utils
{
    internal class DateTimeExtensionTests
    {
        [TestCaseSource(nameof(PreviousMonthTestCases))]
        public void PreviousMonth_ResultsInPreviousMonth(DateTime startMonth, DateTime expectedResult)
        {
            // arrange
            // act
            var result = startMonth.PreviousMonth();

            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCaseSource(nameof(NextMonthTestCases))]
        public void NextMonth_ResultsInPreviousMonth(DateTime startMonth, DateTime expectedResult)
        {
            // arrange
            // act
            var result = startMonth.NextMonth();

            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public static IEnumerable<object[]> PreviousMonthTestCases()
        {
            yield return new object[]{new DateTime(2019, 1, 1), new DateTime(2018, 12, 1)};
            yield return new object[]{new DateTime(2019, 2, 5), new DateTime(2019, 1, 5)};
            yield return new object[]{new DateTime(2019, 3, 1), new DateTime(2019, 2, 1)};
            yield return new object[]{new DateTime(2019, 4, 1), new DateTime(2019, 3, 1)};
            yield return new object[]{new DateTime(2019, 5, 1), new DateTime(2019, 4, 1)};
            yield return new object[]{new DateTime(2019, 6, 1), new DateTime(2019, 5, 1)};
            yield return new object[]{new DateTime(2019, 7, 1), new DateTime(2019, 6, 1)};
            yield return new object[]{new DateTime(2020, 1, 1), new DateTime(2019, 12, 1)};
        }
        
        public static IEnumerable<object[]> NextMonthTestCases()
        {
            return PreviousMonthTestCases().Select(previousMonthTestCase => previousMonthTestCase.Reverse().ToArray());
        }
    }
}
