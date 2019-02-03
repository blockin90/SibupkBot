using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramClientCore.BotCache.Database;
using TelegramClientCore.BotCacheDatabase;
using TelegramClientCore.BotServices;
using UpkModel.Database;
using UpkServices;
using UpkServices.Web;

namespace TelegramClientCore
{
    /// <summary>
    /// Загрузчик расписания с учетом локального кэша
    /// </summary>
    internal class CachedGroupScheduleLoader : IDisposable
    {
        ISiteUpdateNotificator _updatesChecker;
        CacheDbService cacheDb = new CacheDbService();
        Task OutOfDateRecordMonitorTask { get; }

        private static CachedGroupScheduleLoader _instance;
        public static CachedGroupScheduleLoader Instance
        {
            get
            {
                return _instance ?? (_instance = new CachedGroupScheduleLoader());
            }
        }

        private CachedGroupScheduleLoader()
        {
            _updatesChecker = ServiceProvider.GetService<ISiteUpdateNotificator>();
            _updatesChecker.OnSiteUpdate += ResetCache_OnSiteUpdate;
            //запускаем поток на удаление из кэша устаревших записей
            OutOfDateRecordMonitorTask = Task.Run(
                ()=> 
                {
                    TimeSpan sleepInterval = new TimeSpan(24, 0, 0);
                    while (true) {
                        Thread.Sleep(sleepInterval);
                        DateTime date = DateInfo.Today;
                        try {
                            cacheDb.RemoveOutOfDateRecords(date);
                        } catch (Exception e){
                            MyTrace.WriteLine(e.Message);
                        }
                    }
                });
        }

        private void ResetCache_OnSiteUpdate(object sender, EventArgs e)
        {
            cacheDb.ResetCache();
        }

        public IDataLoader<WorkDay> GetDataLoader(Group group, DateTime from, DateTime to)
        {
            return new CachedDataLoader(GetWorkDays(group, from, to));
        }

        public IEnumerable<WorkDay> GetWorkDays(Group group, DateTime from, DateTime to)
        {
            //сначала пробуем загрузить данные с кэша, если успешно - возвращаем их пользователю
            if( cacheDb.TryToGetStudentWorkDays(group.Id, from, to, out IEnumerable<WorkDay> workDays)) {
                return workDays.OrderBy(wd => wd.Date).ToArray();
            } else {//иначе грузим данные с сети
                IEnumerable<WeekInterval> intervals = WeekIntervalsFactory.GetIntervals(DateInfo.Today, to);
                if( intervals != null && intervals.Count() != 0) {
                    //берем максимум данных для сохранения:
                    var loader = new StudentWorkDaysWebLoader(group, intervals, DateInfo.Today, intervals.Last().End);
                    var days = loader.Load();     
                    //после чего возвращаем их пользователю и сохраняем в кэше
                    cacheDb.UpdateStudentCache(group.Id, intervals.Last().End, days);
                    return days.Where(wd => wd.Date >= from && wd.Date <= to).OrderBy(wd => wd.Date).ToArray();
                }
                return null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    cacheDb.Dispose();
                    OutOfDateRecordMonitorTask.Dispose();
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
