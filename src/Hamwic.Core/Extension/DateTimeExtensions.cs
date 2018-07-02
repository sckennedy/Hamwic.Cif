using System;
using System.Collections.Generic;
using Hamwic.Core.Constants;

namespace Hamwic.Core.Extension
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
        private static readonly DateTime MinSqlDate = new DateTime(1754, 1, 1);

        public static DaysOfWeek ToSchedulingDaysOfWeek(this DateTime dateTime)
        {
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return DaysOfWeek.Sunday;
                case DayOfWeek.Monday:
                    return DaysOfWeek.Monday;
                case DayOfWeek.Tuesday:
                    return DaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return DaysOfWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return DaysOfWeek.Thursday;
                case DayOfWeek.Friday:
                    return DaysOfWeek.Friday;
                case DayOfWeek.Saturday:
                    return DaysOfWeek.Saturday;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static int ToUtilDateId(this DateTime dateTime)
        {
            return int.Parse(dateTime.ToString("yyyyMMdd"));
        }

        public static string ToShortDateTimeString(this DateTime dateTime)
        {
            return string.Concat(dateTime.ToShortDateString(), " ", dateTime.ToShortTimeString());
        }

        public static string ToIsoString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        public static string ToIsoStringUtc(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string ToIsoStringNoZone(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static string ToIsoStringWithOffset(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:sszzz");
        }

        public static string ToIsoStringWithOffsetAndSeparator(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static string ToShortDateString(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            return dateTime.Value.ToShortDateString();
        }

        public static string ToShortDateTimeString(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            return dateTime.Value.ToShortDateTimeString();
        }

        public static long ToJavascriptTime(this DateTime dateTime)
        {
            return (long) ((dateTime.ToUniversalTime() - Epoch).TotalMilliseconds + 0.5);
        }

        public static long ToChartDateTime(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(Epoch).TotalMilliseconds;
        }

        public static string ToShortDayDateTimeString(this DateTime dateTime)
        {
            return string.Concat(dateTime.DayOfWeek.ToString().Substring(0, 3), " ", dateTime.ToShortDateString(), " ",
                dateTime.ToShortTimeString());
        }

        public static string ToShortDayDateTimeString(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : dateTime.Value.ToShortDayDateTimeString();
        }

        public static string ToLongDayDateTimeString(this DateTime dateTime)
        {
            return string.Concat(dateTime.DayOfWeek.ToString(), " ", dateTime.ToShortDateString(), " ",
                dateTime.ToShortTimeString());
        }

        public static string ToLongDayDateString(this DateTime dateTime)
        {
            return string.Concat(dateTime.DayOfWeek.ToString(), " ", dateTime.ToShortDateString());
        }

        public static string ToLongDayDateTimeString(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : dateTime.Value.ToLongDayDateTimeString();
        }

        public static string ToShortDayDateString(this DateTime dateTime)
        {
            return string.Concat(dateTime.DayOfWeek.ToString().Substring(0, 3), " ", dateTime.ToShortDateString());
        }

        public static string ToShortDayDateString(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : dateTime.Value.ToShortDayDateString();
        }

        public static DateTime TruncateToMinute(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public static DateTime TruncateToHour(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        }

        public static string ToLongDayDateTimeStringDetailed(this DateTime dateTime)
        {
            return string.Concat(dateTime.ToLongDayDateTimeString(), ":", dateTime.ToString("ss.mmm"));
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }

        public static DateTime Ceiling(this DateTime dt, TimeSpan d)
        {
            return d.Minutes == 0 ? dt : dt.AddMinutes((60 - dt.Minute) % d.Minutes);
        }

        public static DateTime? Ceiling(this DateTime? dt, TimeSpan d)
        {
            return dt == null ? (DateTime?) null : Ceiling(dt.Value, d);
        }

        public static DateTime Floor(this DateTime dt, TimeSpan d)
        {
            return d.Minutes == 0 ? dt : dt.AddMinutes(-dt.Minute % d.Minutes);
        }

        public static DateTime? Floor(this DateTime? dt, TimeSpan d)
        {
            return dt == null ? (DateTime?)null : Floor(dt.Value, d);
        }

        public static DateTime Round(this DateTime dt, TimeSpan d)
        {
            if (d.Minutes == 0)
                return dt;

            var halfTicks = (d.Ticks + 1) >> 1;
            return dt.AddTicks(halfTicks - (dt.Ticks + halfTicks) % d.Ticks);
        }

        public static DateTime? Round(this DateTime? dt, TimeSpan d)
        {
            return dt == null ? (DateTime?)null : Round(dt.Value, d);
        }

        public static DateTime NextInstanceOfDay(this DateTime dateTime, DayOfWeek dayOfWeek)
        {
            return dateTime.LoopToDay(dayOfWeek, 1);
        }

        public static DateTime LastInstanceOfDay(this DateTime dateTime, DayOfWeek dayOfWeek)
        {
            return dateTime.LoopToDay(dayOfWeek, -1);
        }

        public static DateTime LoopToDay(this DateTime dateTime, DayOfWeek dayOfWeek, int increment)
        {
            var dt = dateTime;

            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(increment);

            return dt;
        }

        public static DateTime AddWeekdays(this DateTime dateTime, int numberOfDays)
        {
            var dt = dateTime;
            var days = Math.Abs(numberOfDays);

            while (days > 0)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek != DayOfWeek.Sunday && dt.DayOfWeek != DayOfWeek.Saturday)
                    days--;
            }

            return dt;
        }

        public static IEnumerable<DateTime> DayRange(this DateTime dateTime, int months)
        {
            return DayRange(dateTime, dateTime.AddMonths(months));
        }

        public static IEnumerable<DateTime> DayRange(this DateTime dateTime, DateTime endDate)
        {
            if (endDate < dateTime)
                throw new ArgumentException("Provide the start date from which to calculate a range");

            var dt = dateTime;
            while (dt <= endDate)
            {
                yield return dt;
                dt = dt.AddDays(1);
            }
        }

        public static DateTime? RelativeTo(this DateTime time, DateTime relativeTo)
        {
            return ((DateTime?)time).RelativeTo(relativeTo);
        }

        public static DateTime? RelativeTo(this DateTime? time, DateTime? relativeTo)
        {
            if (!time.HasValue)
                return null;

            if (!relativeTo.HasValue)
                return time;

            return new DateTime(relativeTo.Value.Year,
                relativeTo.Value.Month,
                relativeTo.Value.Day,
                time.Value.Hour,
                time.Value.Minute,
                0);
        }

        public static DateTime? RelativeToAfter(this DateTime? time, DateTime? relativeTo)
        {
            var result = time.RelativeTo(relativeTo);
            if (!result.HasValue)
                return null;
            if (!relativeTo.HasValue)
                return time;

            return result < relativeTo.Value ? result.Value.AddDays(1) : result;
        }

        public static DateTime? RelativeToWithin(this DateTime? time, DateTime? relativeTo, int hoursTolerance = 6)
        {
            var result = time.RelativeTo(relativeTo);
            if (!result.HasValue)
                return null;
            if (!relativeTo.HasValue)
                return time;

            if (result < relativeTo.Value.AddHours(Math.Abs(hoursTolerance) * -1))
                return result.Value.AddDays(1);

            if (result > relativeTo.Value.AddHours(Math.Abs(hoursTolerance)))
                return result.Value.AddDays(-1);

            return result;
        }

        public static DateTime? RelativeToBefore(this DateTime? time, DateTime? relativeTo)
        {
            var result = time.RelativeTo(relativeTo);
            if (!result.HasValue)
                return null;
            if (!relativeTo.HasValue)
                return time;

            return result > relativeTo.Value ? result.Value.AddDays(-1) : result;
        }

        public static DateTime Add(this DateTime dateTime, PeriodType periodType, double number = 1)
        {
            switch (periodType)
            {
                case PeriodType.Days:
                    return dateTime.AddDays(number);
                case PeriodType.Weeks:
                    return dateTime.AddDays(number * 7);
                case PeriodType.Months:
                    return dateTime.AddMonths((int)number);
                case PeriodType.Years:
                    return dateTime.AddYears((int)number);
                default:
                    throw new ArgumentOutOfRangeException(nameof(periodType), periodType, null);
            }
        }

        public static bool IsValidSqlDate(this DateTime? dateTime)
        {
            return dateTime.HasValue && dateTime.Value > MinSqlDate;
        }

        public static string TimeFromExcelDate(this string timeString)
        {
            return DateFromExcelDate(timeString)?.ToShortTimeString();
        }

        public static DateTime? DateFromExcelDateAndTime(this string dateString, string timeString, DateTime? occursAfter = null)
        {
            var date = DateFromExcelDate(dateString);
            var time = TimeFromExcelDate(timeString);
            if (date.HasValue && !string.IsNullOrEmpty(time))
            {
                var result = date.Value.Date.Add(TimeSpan.Parse(time));
                return occursAfter != null && result < occursAfter ? result.AddDays(1) : result;
            }

            return null;
        }

        public static DateTime? DateFromExcelDate(this string s)
        {
            if (double.TryParse(s, out var d))
            {
                return DateTime.FromOADate(d);
            }

            return null;
        }

        public static string ParseTimeStringFromDouble(string timeString)
        {
            var result = timeString;

            if (double.TryParse(timeString, out double timeDouble))
            {
                result = DateTime.FromOADate(timeDouble).ToShortTimeString();
            }

            return result;
        }

    }
}