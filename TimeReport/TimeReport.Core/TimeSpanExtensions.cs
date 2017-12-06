using System;
using System.Collections.Generic;

namespace TimeReport.Core
{
    public static class TimeSpanExtensions
    {
        public static string ToHumanReadable(this TimeSpan timeSpan)
        {
            var parts = new List<string>();
            var negative = false;

            if (timeSpan.Ticks < 0)
            {
                negative = true;
                timeSpan = timeSpan.Negate();
            }

            if (timeSpan.Days > 0)
                parts.Add(string.Format("{0}d", timeSpan.Days));

            if (timeSpan.Hours > 0)
                parts.Add(string.Format("{0}h", timeSpan.Hours));

            if (timeSpan.Minutes > 0)
                parts.Add(string.Format("{0}m", timeSpan.Minutes));

            if (parts.Count > 0)
                return (negative ? "-" : "") + string.Join(" ", parts);

            return "0m";
        }
    }
}
