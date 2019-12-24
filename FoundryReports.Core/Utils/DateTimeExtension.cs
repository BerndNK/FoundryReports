using System;

namespace FoundryReports.Core.Utils
{
    public static class DateTimeExtension
    {
        public static DateTime PreviousMonth(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var targetMonth = dateTime.Month - 1;
            if(targetMonth <= 0)
            {
                targetMonth = 12;
                year -= 1;
            }

            return new DateTime(year, targetMonth, dateTime.Day);
        }
        
        public static DateTime NextMonth(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var targetMonth = dateTime.Month + 1;
            if(targetMonth > 12)
            {
                targetMonth = 1;
                year += 1;
            }

            return new DateTime(year, targetMonth, dateTime.Day);
        }
    }
}
