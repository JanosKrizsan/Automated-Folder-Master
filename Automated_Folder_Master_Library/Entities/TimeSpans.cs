using System;
using System.Collections.Generic;
using System.Text;

namespace Master_Library.Services
{
    public struct TimeSpans
    {
        public static TimeSpan OneWeek = TimeSpan.FromDays(7);
        public static TimeSpan TwoWeeks = TimeSpan.FromDays(14);
        public static TimeSpan OneMonth = TimeSpan.FromDays(30);
        public static TimeSpan ThreeMonths = TimeSpan.FromDays(90);
        public static TimeSpan SixMonths = TimeSpan.FromDays(180);
        public static TimeSpan OneYear = TimeSpan.FromDays(365);
    }
}
