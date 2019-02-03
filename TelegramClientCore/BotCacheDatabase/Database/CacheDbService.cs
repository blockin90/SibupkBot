using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.BotCache.Database
{
    /// <summary>
    /// сервис для работы с кэшированной БД, хранящей расписание и размещенной в памяти
    /// </summary>
    public class CacheDbService : IDisposable
    {
        private CacheDbContext _context;
        private object syncObject = new object();


        public CacheDbService()
        {
            _context = new CacheDbContext();
        }
        public CacheDbService(CacheDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Попытка получить расписание для заданной группы из локального кэша на заданный интервал, 
        /// начиная с текущего дня
        /// </summary>
        /// <param name="groupId">id группы, для которой требуется загрузить расписание</param>
        /// <param name="to">последний день интервала, для которого грузится расписание</param>
        /// <param name="workDays">результирующая последовательность дней, 
        /// либо null, если кэш не содержит информации по группе</param>
        /// <returns>true, если в БД содержится информация по данной группе до заданной даты</returns>
        public bool TryToGetStudentWorkDays(int groupId, DateTime from, DateTime to, out IEnumerable<WorkDay> workDays)
        {
            workDays = null;
            lock (syncObject) {
                //если отсутствует группа - искать дальше смысла нет
                var group = _context.CachedGroups.Find(groupId);
                //кроме наличия группы проверяем, чтобы последняя загруженная дата для этой группы была
                //не меньше запрашиваемой
                if (group != null && group.CachedTo >= to) {
                    workDays = group.CachedGroupWorkDays
                        .Where(wd => wd.Date >= from && wd.Date <= to)
                        .Select( cwd => cwd.WorkDay)
                        .OrderBy( wd => wd.Date)
                        .ToArray();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Обновление кэша расписания для выбранной группы
        /// </summary>
        /// <param name="groupId">группа, для которой обновляется расписание</param>
        /// <param name="maxDate">максимальная дата для которой было запрошено(!) расписание. 
        /// Может отличаться от максимального значения, хранимого в workDays.</param>
        /// <param name="workDays">упорядоченная последовательность дней расписания, которые требуется загрузить в кэш</param>
        public void UpdateStudentCache(int groupId, DateTime maxDate, IEnumerable<WorkDay> workDays)
        {
            //DateTime maxDate = workDays.Max(wd => wd.Date).Date;
            lock (syncObject) {
                var group = _context.CachedGroups.Find(groupId);
                if (group == null) {//если группа отсутствует - создаем новую
                    CreateCacheForGroup(groupId, maxDate, workDays);
                } else if (group.CachedTo < maxDate) {
                    //добавляем в БД только те записи, которые старше хранимого в group.CachedTo значения
                    _context.CachedGroupWorkDays.AddRange(workDays
                        .Where(wd => wd.Date > group.CachedTo)
                        .Select(wd => new CachedGroupWorkDay(groupId, wd)));
                    group.CachedTo = maxDate;
                    _context.SaveChanges();
                }
                //сюда попадаем если максимальная хранимая дата больше, чем та, 
                //которую предлагается сохранить
                //выходим без обновления т.к. хранимый интервал больше                
            }
        }
        /// <summary>
        /// Создание кэша для новой группы
        /// </summary>
        private void CreateCacheForGroup(int groupId, DateTime maxDate, IEnumerable<WorkDay> workDays)
        {
            CachedGroup group = new CachedGroup()
            {
                CachedGroupId = groupId,
                CachedTo = maxDate,
                CachedGroupWorkDays = workDays.Select(wd => new CachedGroupWorkDay(groupId, wd)).ToList()
            };
            lock (syncObject) {
                _context.CachedGroups.Add(group);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить все записи, старше date
        /// </summary>
        /// <param name="date">дата, старше которой данные будут считаться неактуальными
        /// и подлежать удалению
        /// </param>
        public void RemoveOutOfDateRecords( DateTime date )
        {
            lock (syncObject) {
                //правильнее было бы послать SQL запрос, но в этом случае не обновляются
                //соответствующие DbSet'ы и перезагружать все будет затратнее
                var removedEntries = _context.WorkDays.Where(wd => wd.Date < date);
                _context.WorkDays.RemoveRange(removedEntries);
                /*_context.Database.ExecuteSqlCommand("delete from WorkDays where Date < @date",
                    new SqliteParameter("date", date));*/
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// сбросить кэш, очистить БД
        /// </summary>
        public void ResetCache()
        {
            lock (syncObject) {
                _context.Dispose();
                _context = new CacheDbContext();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    _context.Dispose();
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
