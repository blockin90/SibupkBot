using System;
using System.Collections.Generic;
using System.Text;
using UpkModel.Database;
using UpkServices.Web;

namespace UpkServices
{
    public class StudentWorkDaysLoaderFactory
    {
        private readonly UpkDatabaseContext _database;
        private readonly Configs _configs;

        public StudentWorkDaysLoaderFactory(UpkDatabaseContext database, Configs configs)
        {
            _database = database;
            _configs = configs;
        }

        /// <summary>
        /// Получение загрузчика расписания для преподавателя на заданные диапазон дат
        /// </summary>
        /// <param name="group">группа, для которой требуется загрузить расписание</param>
        /// <param name="from">начальная дата диапазона</param>
        /// <param name="to">последняя дата диапазона</param>
        /// <remarks>
        /// Позволяет получить доступный в текущих условиях загрузчик расписания
        /// Если локальный кэш в базе данных позволяет получить требуемые данные, возвращается ссылка на него
        /// Иначе загрузка идет из интернета, с сайта СибУПК
        /// 
        /// </remarks>
        public IDataLoader<WorkDay> GetLoader(Group group, DateTime from, DateTime to)
        {
            IEnumerable<WeekInterval> intervals = WeekIntervalsFactory.GetIntervals(from, to);
            return new StudentWorkDaysWebLoader(group, intervals, from, to);
        }
    }
}
