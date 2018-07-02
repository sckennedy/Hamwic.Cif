using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hamwic.Core.Extension
{
    public static class NumericExtensions
    {
        private static readonly NumberFormatInfo CurrencyFormatNoSymbol;

        static NumericExtensions()
        {
            CurrencyFormatNoSymbol = CultureInfo.CurrentCulture.NumberFormat;
            CurrencyFormatNoSymbol = (NumberFormatInfo) CurrencyFormatNoSymbol.Clone();
            CurrencyFormatNoSymbol.CurrencySymbol = "";
        }

        public static string ToCurrencyStringNoSymbol(this decimal v)
        {
            return string.Format(CurrencyFormatNoSymbol, "{0:c}", v);
        }

        public static string ToCurrencyStringWithSymbol(this decimal v)
        {
            //TODO - extend this so we can get the culture specific to the supplier or customer
            return ToCurrencyStringWithSymbol(v, "en-GB");
        }

        public static string ToCurrencyStringWithSymbol(this decimal v, string culture)
        {
            var currencyFormat = CultureInfo.CreateSpecificCulture(culture).NumberFormat;
            return string.Format(currencyFormat, "{0:c}", v);
        }

        public static int RoundUpToNext(this int n, int roundTo)
        {
            return (int) (Math.Ceiling(n / (decimal) roundTo) * roundTo);
        }

        public static int RoundUpToNext(this decimal n, int roundTo)
        {
            return (int) (Math.Ceiling(n / roundTo) * roundTo);
        }

        public static decimal TruncateToDecimalPlaces(this decimal n, int decimalPlaces)
        {
            var step = (decimal) Math.Pow(10, decimalPlaces);
            var tmp = (int) Math.Truncate(step * n);
            return tmp / step;
        }

        public static string NeedsS(this int n)
        {
            return n == 1 ? "" : "s";
        }

        public static double ToTravelTimeSeconds(this float? n, double defaultValue, double speedMph = 22)
        {
            if (!n.HasValue)
                return defaultValue;

            return (double) (n / speedMph) * 60 * 60;
        }

        public static float MetresToMiles(this int m)
        {
            return (float) (m / 1609.344);
        }

        public static string MinutesAsTimeString(this int n)
        {
            var span = TimeSpan.FromMinutes(Math.Abs(n));
            var parts = new List<string>();
            if (span.Days > 0)
                parts.Add(span.Days + " day" + span.Days.NeedsS());

            if (span.Hours > 0)
                parts.Add(span.Hours + " hr" + span.Hours.NeedsS());

            if (span.Minutes > 0 || n == 0)
                parts.Add(span.Minutes + " min" + span.Minutes.NeedsS());

            return (n < 0 ? "-" : "") + string.Join(" ", parts);
        }

        public static string SecondsAsTimeString(this int n)
        {
            return MinutesAsTimeString(n / 60);
        }

        public static string Times(this int n, string msg)
        {
            msg = (msg ?? "").TrimEnd();
            switch (n)
            {
                case 0:
                    return $"Not {msg}";
                case 1:
                    return msg;
                case 2:
                    return $"{msg} twice".TrimStart();
                default:
                    return $"{msg} {n} times".TrimStart();
            }
        }
    }
}