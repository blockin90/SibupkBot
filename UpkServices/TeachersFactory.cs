using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices.Web;

namespace UpkServices
{
    /// <summary>
    /// Класс-"производитель" преподавателей. По возможности использует локальную БД, если 
    /// в ней нет данных, подгружает с сайта и сохраняет в БД.
    /// </summary>
    public class TeachersFactory
    {
        private readonly UpkDatabaseContext _database;

        public TeachersFactory( UpkDatabaseContext database)
        {
            _database = database;
        }
        /// <summary>
        /// Загрузка списка преподавателей, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public Teacher[] GetTeachers( bool forceFromWeb = false)
        {
            if( forceFromWeb || !_database.Teachers.Any()) {
                LoadFromWeb();
            }
            return _database.Teachers.Include(t => t.Department).AsNoTracking().OrderBy( t => t.FIO).ToArray();
        }
        /// <summary>
        /// Асинхронная загрузка списка преподавателей, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public async Task<Teacher[]> GetTeachersAsync(bool forceFromWeb = false)
        {
            if (forceFromWeb || !  _database.Teachers.Any()) {
                await Task.Run(() => GetTeachers(forceFromWeb));
            }
            return _database.Teachers.AsNoTracking().OrderBy(t => t.FIO).ToArray();
        }
        private void LoadFromWeb()
        {
            //перед загрузкой преподавателей необходимо загрузить список кафедр
            var loader = new RepeatableObjectWebLoader(Settings.MaxAttempts, Settings.RetryInterval);
            var departments = loader.Load<Department>();
            var teachers = new RepeatableObjectWebLoader(Settings.MaxAttempts, Settings.RetryInterval).Load<Teacher>();
            /* на сайте могут не совпадать id кафедр из списка преподавателей со списком кафедр,
             * поэтому для таких кафедр формируем фиктивное название */
            var deptsId = departments.Select(d => d.Id);
            var teachersDeptsId = teachers.Select(d => (int) d.DepartmentId);
            var fakeDeptsId = teachersDeptsId.Except(deptsId);
            var fakeDepts = fakeDeptsId.Select(id => new Department() { Id = id, Name = "Unknown department" });
            departments = departments.Union(fakeDepts);
            //сопоставляем преподавателей и кафдеры и сохраняем в БД
            teachers = teachers.Join(departments, t => t.DepartmentId, d => d.Id, (t, d) => { t.Department = d; return t; });

            SaveTeachersChanges(teachers);
            //return teachers.OrderBy(t => t.FIO).ToArray();
        }
        private void SaveTeachersChanges(IEnumerable<Teacher> teachers)
        {
            _database.UpdateTeachers(teachers);
        }
    }
}
