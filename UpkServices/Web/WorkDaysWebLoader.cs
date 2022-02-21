using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpkModel.Database.Schedule;

namespace UpkServices.Web
{
    public abstract class WorkDaysWebLoader : IDataLoader<WorkDay>
    {
        protected readonly IEnumerable<WeekInterval> _intervalsToLoad;
        protected readonly DateTime _from;
        protected readonly DateTime _to;
        protected readonly int _maxAttempts;
        protected readonly int _sleepInterval;
        /// <summary>
        /// Конструктор класса TeacherWorkDaysWebLoader
        /// </summary>
        /// <param name="intervalsToLoad">список недельных интервалов вида 20.02.2018 - 02.03.2018 для загрузки</param>
        /// <param name="from">Дата, ниже которой все результаты будут отброшены</param>
        /// <param name="to">Дата, после которой все результаты будут отброшены</param>
        /// <param name="maxAttempts">Максимальное количество попыток загрузки</param>
        /// <param name="sleepInterval">Время ожидания между загрузками</param>
        protected WorkDaysWebLoader(IEnumerable<WeekInterval> intervalsToLoad, DateTime from, DateTime to, int maxAttempts = 10, int sleepInterval = 200)
        {
            _intervalsToLoad = intervalsToLoad;
            _from = from;
            _to = to;
            _maxAttempts = maxAttempts;
            _sleepInterval = sleepInterval;
        }
        /// <summary>
        /// Функция получения дней расписаний. Способ получения зависит от того, для кого загружается расписание.
        /// </summary>
        /// <returns></returns>
        protected abstract WorkDay[] GetWorkDays();
        
        public IEnumerable<WorkDay> Load()
        {
            var res = GetWorkDays();
            return res.SkipWhile(wd => wd.Date < _from)
                      .TakeWhile(wd => wd.Date <= _to)
                      .ToArray();
        }
        public Task<IEnumerable<WorkDay>> LoadAsync()
        {
            return Task.Run(() => Load());
        }
    }
}
