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
    /// Класс-"производитель" списка факультетов. По возможности использует локальную БД, если 
    /// в ней нет данных, подгружает с сайта и сохраняет в БД.
    /// </summary>
    class FacultiesFactory
    {
        private readonly UpkDatabaseContext _database;       

        public FacultiesFactory(UpkDatabaseContext database)
        {
            _database = database;
        }
        /// <summary>
        /// Загрузка списка факультетов, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public Faculty[] GetFaculties(bool forceFromWeb = false)
        {
            if (forceFromWeb || !_database.Faculties.Any()) {
                LoadFromWeb();
            }
            return _database.Faculties.OrderBy(t => t.Name).ToArray();
        }
        /// <summary>
        /// Асинхронная загрузка списка преподавателей, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public async Task<Faculty[]> GetFacultiesAsync(bool forceFromWeb = false)
        {
            if (forceFromWeb || !_database.Faculties.Any()) {
                await Task.Run(() => GetFaculties(forceFromWeb));
            }
            return _database.Faculties.AsNoTracking().OrderBy(t => t.Name).ToArray();
        }
        private void LoadFromWeb()
        {
            //перед загрузкой преподавателей необходимо загрузить список кафедр
            var loader = new RepeatableObjectWebLoader(Settings.MaxAttempts, Settings.RetryInterval);
            var departments = loader.Load<Department>();
            var teachers = new RepeatableObjectWebLoader(Settings.MaxAttempts, Settings.RetryInterval).Load<Teacher>();
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
