using System;
using System.Collections.Generic;
using System.Linq;
using UpkModel.Database;

namespace UpkServices.Web
{
    public class StudentWorkDaysWebLoader : WorkDaysWebLoader
    {
        private readonly Group group;

        /// <summary>
        /// Конструктор класса StudentWorkDaysWebLoader
        /// </summary>
        /// <param name="teacher">Преподаватель, для которого необходимо загрузить данные</param>
        /// <param name="intervalsToLoad">список недельных интервалов вида 20.02.2018 - 02.03.2018 для загрузки</param>
        /// <param name="from">Дата, ниже которой все результаты будут отброшены</param>
        /// <param name="to">Дата, после которой все результаты будут отброшены</param>
        /// <param name="maxAttempts">Максимальное количество попыток загрузки</param>
        /// <param name="sleepInterval">Время ожидания между загрузками</param>
        public StudentWorkDaysWebLoader(Group group, IEnumerable<WeekInterval> intervalsToLoad, DateTime from, DateTime to, int maxAttempts = 10, int sleepInterval = 200)
            : base(intervalsToLoad, from, to, maxAttempts, sleepInterval)
        {
            this.group = group;
        }
        protected override WorkDay[] GetWorkDays()
        {
            int kurs = group.Kurs;  //чтобы избежать множественных расчетов
            return _intervalsToLoad.SelectMany(
                ti =>
                {
                    string postData = String.Format(
                        HtmlNodeParsers.StudentSchedulePostDataTemplate,
                        group.id_Forma,
                        group.id_Fak,
                        kurs,
                        group.NamePodGrup,
                        ti.OriginalString);
                    return new RepeatableObjectWebLoader(_maxAttempts, _sleepInterval)
                        .Load<StudentWorkDay>(postData);
                })
                .ToArray();
        }
    }
}
