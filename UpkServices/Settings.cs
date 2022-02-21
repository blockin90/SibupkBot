using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UpkModel.Database;
using UpkModel.Database.Schedule;

namespace UpkServices
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    public class Settings
    {
        protected Settings()
        {
        }

        public static int MaxAttempts { get; } = 5;
        public static int RetryInterval { get; } = 300;
        
        public static TimeSpan ScheduleRequestTimeOut
        {
            get
            {
#if DEBUG
                if (Debugger.IsAttached) {
                    return TimeSpan.FromMinutes(5);
                }
#endif
                return TimeSpan.FromSeconds(15);

            }
        }
        public static TimeSpan ReportRequestTimeOut
        {
            get
            {
#if DEBUG
                if (Debugger.IsAttached) {
                    return TimeSpan.FromMinutes(5);
                }
#endif
                return TimeSpan.FromSeconds(30);

            }
        }

        public static TimeSpan CacheCleanupInterval
        {
            get
            {
                return TimeSpan.FromDays(1);
            }
        }
    }
}
