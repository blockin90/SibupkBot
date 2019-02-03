using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpkModel.Database;

namespace UpkServices.Web
{
    /// <summary>
    /// Загрузчик недельных интервалов с сайта СибУПК
    /// </summary>
    /// <remarks>
    /// на момент использования данного класса должны быть загружены преподаватели в БД
    /// </remarks>
    public static class WeekIntervalsFactory
    {
        //todo: добавить время обновления интервалов, чтобы исключить слишком частые обращения к сайту,и сохранить это как параметр системы

        static Teacher _defaultTeacher;

        /// <summary>
        /// Преподаватель по умолчанию
        /// </summary>
        /// <remarks>
        /// Недельные интервалы берутся со странички расписания преподавателей. Здесь указан рандомный преподаватель
        /// для которого будут составляться запросы.
        /// </remarks>
        private static Teacher DefaultTeacher
        {
            get
            {
                return _defaultTeacher ?? (_defaultTeacher = UpkDatabaseContext
                    .Instance
                    .Teachers
                    .FirstOrDefault( t => t.FIO == "Блок Иван Николаевич"));
            }
        }

        /// <summary>
        /// Загрузка списка интервалов с сайта СибУПК, и попытка определить, 
        /// на какие из них попадают заданные даты
        /// </summary>
        public static IEnumerable<WeekInterval> GetIntervals(DateTime from, DateTime to)
        {
            if (TryToGetIntervalsFromDb(out IEnumerable<WeekInterval> intervals, from, to)) {
                return intervals;
            }
            return GetIntervalsFromWeb(from, to, DefaultTeacher);
        }
        /// <summary>
        /// Загрузка списка недельных интервалов с сайта СибУПК и сохранение их в БД
        /// </summary>
        static IEnumerable<WeekInterval> GetIntervalsFromWeb(DateTime from, DateTime to, Teacher teacher)
        {
            var allIntervals = new RepeatableObjectWebLoader(Configs.Instance.MaxAttempts, Configs.Instance.SleepInterval)
                .Load<WeekInterval>(String.Format(HtmlNodeParsers.DateIntervalsPostDataTemplate, teacher.DepartmentId, teacher.FIO))
                .OrderBy(wi => wi.Start);
            SaveWeekIntervalsChanges(allIntervals);
            return allIntervals
                .SkipWhile(wi => wi.End < from)
                .TakeWhile(wi => wi.Start <= to)
                .ToArray();
        }
        /// <summary>
        /// Поиск интервалов, на которые попадают заданные даты и попытка загрузки их из БД
        /// </summary>
        static bool TryToGetIntervalsFromDb(out IEnumerable<WeekInterval> result, DateTime from, DateTime to)
        {
            result = null;
            lock (UpkDatabaseContext.Instance.WeekIntervals) {
                var first = UpkDatabaseContext.Instance.WeekIntervals.LastOrDefault(wi => wi.Start <= from);
                var last = UpkDatabaseContext.Instance.WeekIntervals.FirstOrDefault(wi => to <= wi.End);
                if (first == null || last == null) {
                    return false;
                }
                result = UpkDatabaseContext.Instance.WeekIntervals
                    .Where(wi => wi.End >= from)
                    .Where(wi => wi.Start <= to)
                    .OrderBy(wi => wi.Start)
                    .ToArray();
            }
            return true;
        }
        static void SaveWeekIntervalsChanges(IEnumerable<WeekInterval> weekIntervals)
        {
            UpkDatabaseContext.Instance.UpdateWeekIntervals(weekIntervals);
        }
    }
}
