using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices.Web;

namespace UpkServices
{
    /// <summary>
    /// Загрузчик расписания (рабочих дней) для преподавателей
    /// </summary>
    /// <remarks>
    /// на текущий момент использует только загрузку с сайта
    /// </remarks>
    public class TeacherWorkDaysLoaderFactory
    {
        private readonly UpkDatabaseContext _database;

        public TeacherWorkDaysLoaderFactory(UpkDatabaseContext database)
        {
            _database = database;
        }

        /// <summary>
        /// Получение загрузчика расписания для преподавателя на заданные диапазон дат
        /// </summary>
        /// <param name="teacher">преподаватель, для которого требуется загрузить расписание</param>
        /// <param name="from">начальная дата диапазона</param>
        /// <param name="to">последняя дата диапазона</param>
        /// <remarks>
        /// Позволяет получить доступный в текущих условиях загрузчик расписания
        /// Если локальный кэш в базе данных позволяет получить требуемые данные, возвращается ссылка на него
        /// Иначе загрузка идет из интернета, с сайта СибУПК
        /// 
        /// </remarks>
        public IDataLoader<WorkDay> GetLoader(Teacher teacher, DateTime from, DateTime to)
        {
            IEnumerable<WeekInterval> intervals = WeekIntervalsFactory.GetIntervals(from, to);
            return new TeacherWorkDaysWebLoader(teacher, intervals, from, to);
        }
    }
}
