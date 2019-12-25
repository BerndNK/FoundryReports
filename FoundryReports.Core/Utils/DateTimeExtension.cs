using System;

namespace FoundryReports.Core.Utils
{
    public static class DateTimeExtension
    {
        public static DateTime PreviousMonth(this DateTime dateTime)
        {
            return PreviousMonths(dateTime, 1);
        }

        public static DateTime NextMonth(this DateTime dateTime)
        {
            return NextMonths(dateTime, 1);
        }

        public static DateTime PreviousMonths(this DateTime dateTime, int months)
        {
            var year = dateTime.Year;
            var targetMonth = dateTime.Month - months;
            while (targetMonth <= 0)
            {
                targetMonth = 12 + targetMonth;
                year -= 1;
            }

            return new DateTime(year, targetMonth, dateTime.Day);
        }

        public static DateTime NextMonths(this DateTime dateTime, int months)
        {
            var year = dateTime.Year;
            var targetMonth = dateTime.Month + months;
            while (targetMonth > 12)
            {
                targetMonth = targetMonth-12;
                year += 1;
            }

            return new DateTime(year, targetMonth, dateTime.Day);
        }
    }
}