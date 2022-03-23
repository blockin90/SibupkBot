using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramClientCore.BotServices;
using UpkModel.Database.Schedule;
using UpkServices;
using UpkServices.Web;

namespace TelegramClientCore.BotCache
{
    /// <summary>
    /// Загрузчик расписания с учетом локального кэша
    /// </summary>
    internal class GroupScheduleCache : IDisposable
    {
        static object _syncObject = new object();

        ConcurrentDictionary<int, MemoryCache> _groupsCache;

        public GroupScheduleCache()
        {
            _groupsCache = new ConcurrentDictionary<int, MemoryCache>();
        }

        public void Reset()
        {
            try {
                var oldCache = _groupsCache;
                var newCache = new ConcurrentDictionary<int, MemoryCache>();
                _groupsCache = newCache;
                foreach (var record in oldCache) {
                    record.Value.Dispose();
                }
            } catch (Exception e) {
                MyTrace.WriteLine(e.GetFullMessage());
            }
        }

        private MemoryCache CreateEmptyCache()
        {
            var options = new MemoryCacheOptions()
            {
                ExpirationScanFrequency = Settings.CacheCleanupInterval
            };
            return new MemoryCache(options);
        }
        private void CacheWorkDay(MemoryCache memoryCache, WorkDay workDay)
        {
            memoryCache.Set(workDay.Date, workDay, workDay.Date.AddDays(1));
        }

        public IEnumerable<WorkDay> GetWorkDays(Group group, DateTime from, DateTime to)
        {
            var cache = _groupsCache.GetOrAdd(group.Id, id => CreateEmptyCache());
            var result = new List<WorkDay>();
            if(from < DateInfo.Today) {
                from = DateInfo.Today;
            }
            for (var date = from; date <= to; date = date.AddDays(1)) {
                WorkDay workDay = null;
                if (!cache.TryGetValue(date, out workDay)) {
                    IEnumerable<WeekInterval> intervals = WeekIntervalsFactory.GetIntervals(DateInfo.Today, to);
                    if (intervals != null && intervals.Count() != 0) {
                        //обновляем кэш на запрашиваемые даты
                        var lastIntervalDate = intervals.Last().End;
                        var loader = new StudentWorkDaysWebLoader(group, intervals, date, lastIntervalDate);
                        var workDays = loader.Load();
                        var missingDate = date;
                        var enumerator = workDays.GetEnumerator();
                        //заполняем "пустоты" в расписании
                        while(missingDate <= lastIntervalDate) {
                            if (enumerator.MoveNext()) {
                                while (enumerator.Current.Date > missingDate) {
                                    CacheWorkDay( cache, new WorkDay(missingDate));
                                    missingDate = missingDate.AddDays(1);
                                }
                                CacheWorkDay(cache, enumerator.Current);
                            } else {
                                CacheWorkDay(cache, new WorkDay(missingDate));
                            }
                            missingDate = missingDate.AddDays(1);
                        }
                        cache.TryGetValue(date, out workDay);
                    }
                }
                result.Add(workDay);
            }
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    var oldCache = _groupsCache;
                    foreach (var record in oldCache) {
                        record.Value.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
