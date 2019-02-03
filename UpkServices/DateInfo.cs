using System;
using System.Collections.Generic;
using System.Text;

namespace UpkServices
{
    /// <summary>
    /// Класс для определения текущего времени, даты в часовой зоне +7 (Новосибирск)
    /// </summary>
    public static class DateInfo
    {
        static TimeZoneInfo _nskTimeZone;

        public static TimeZoneInfo NskTimeZone
        {
            get => _nskTimeZone ?? ( _nskTimeZone = TimeZoneInfo.CreateCustomTimeZone("nsk time zone", new TimeSpan(7,0,0),"nsk time zone", "nsk time zone"));
        }

        public static DateTime Today
        {
            get
            {
                var utcTime = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, NskTimeZone).Date;
            }
        }
        public static DateTime Now
        {
            get
            {
                var utcTime = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, NskTimeZone);
            }
        }
    }
}
