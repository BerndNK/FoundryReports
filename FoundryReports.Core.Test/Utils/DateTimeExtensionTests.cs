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

        
        [TestCaseSource(nameof(PreviousMonthsTestCases))]
        public void PreviousMonths_ResultsInPreviousMonth(DateTime startMonth, DateTime expectedResult, int amountOfMonths)
        {
            // arrange
            // act
            var result = startMonth.PreviousMonths(amountOfMonths);

            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(NextMonthsTestCases))]
        public void NextMonths_ResultsInNextMonth(DateTime startMonth, DateTime expectedResult, int amountOfMonths)
        {
            // arrange
            // act
            var result = startMonth.NextMonths(amountOfMonths);

            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public static IEnumerable<object[]> PreviousMonthsTestCases()
        {
            yield return new object[]{new DateTime(2019, 1, 1), new DateTime(2018, 12, 1), 1};
            yield return new object[]{new DateTime(2019, 2, 5), new DateTime(2018, 2, 5), 12};
            yield return new object[]{new DateTime(2019, 3, 1), new DateTime(2017, 2, 1), 25};
        }

        public static IEnumerable<object[]> NextMonthsTestCases()
        {
            return PreviousMonthsTestCases().Select(t => new object[] {t[1], t[0], t[2]});
        }
    }
}
