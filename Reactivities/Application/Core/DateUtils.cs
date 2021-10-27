using System;

namespace ExtensionMethods
{
    public static class DateExtensions
    {
        public static DateTime GetStartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        
        public static DateTime GetEndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
    }
}