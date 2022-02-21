using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpkModel.Database.Schedule;

namespace UpkServices.Web
{
    /// <summary>
    /// Загрузчик расписания на преподавателя с сайта СибУПК
    /// </summary>
    internal class TeacherWorkDaysWebLoader : WorkDaysWebLoader
    {
        private readonly Teacher _teacher;
        /// <summary>
        /// Конструктор класса TeacherWorkDaysWebLoader
        /// </summary>
        /// <param name="teacher">Преподаватель, для которого необходимо загрузить данные</param>
        /// <param name="intervalsToLoad">список недельных интервалов вида 20.02.2018 - 02.03.2018 для загрузки</param>
        /// <param name="from">Дата, ниже которой все результаты будут отброшены</param>
        /// <param name="to">Дата, после которой все результаты будут отброшены</param>
        /// <param name="maxAttempts">Максимальное количество попыток загрузки</param>
        /// <param name="sleepInterval">Время ожидания между загрузками</param>
        public TeacherWorkDaysWebLoader(Teacher teacher, IEnumerable<WeekInterval> intervalsToLoad, DateTime from, DateTime to, int maxAttempts = 10, int sleepInterval = 200)
            : base(intervalsToLoad, from, to, maxAttempts, sleepInterval)
        {
            _teacher = teacher;

        }
        
        protected override WorkDay[] GetWorkDays()
        {
            return _intervalsToLoad.SelectMany(
                ti =>
                {
                    string postData = String.Format(
                        HtmlNodeParsers.SchedulePostDataTemplate,
                        _teacher.DepartmentId,
                        _teacher.FIO,
                        ti.OriginalString);
                    return new RepeatableObjectWebLoader(_maxAttempts, _sleepInterval).Load<WorkDay>(postData);
                })
                .ToArray();
        }
    }
}
