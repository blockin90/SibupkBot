using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpkModel.Database;
using UpkServices.Web;

namespace UpkServices
{
    /// <summary>
    /// Класс-"производитель" преподавателей. По возможности использует локальную БД, если 
    /// в ней нет данных, подгружает с сайта и сохраняет в БД.
    /// </summary>
    public class GroupsFactory
    {
        private readonly UpkDatabaseContext _database;
        private readonly Configs _configs;

        public GroupsFactory(UpkDatabaseContext database, Configs configs)
        {
            _database = database;
            _configs = configs;
        }
        /// <summary>
        /// Загрузка списка групп, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public Group[] GetGroups(bool forceFromWeb = false)
        {
            if (forceFromWeb || !_database.Groups.Any()) {
                LoadFromWeb();
            }
            return _database.Groups.AsNoTracking().OrderBy(t => t.ShortName).ToArray();
        }
        /// <summary>
        /// Асинхронная загрузка списка групп, если возможно - из локальной БД, иначе - с сайта СибУПК
        /// </summary>
        /// <param name="force">Принудительная загрузка с сайта СибУПК</param>
        public async Task<Group[]> GetGroupsAsync(bool forceFromWeb = false)
        {
            if (forceFromWeb || !_database.Groups.Any()) {
                await Task.Run(() => GetGroups(forceFromWeb));
            }
            return _database.Groups.AsNoTracking().OrderBy(t => t.ShortName).ToArray();
        }
        private void LoadFromWeb()
        {
            /* для загрузки списка групп нужно учесть:
             * 1)форму обучения, возможные значения 1,2,4  - на всякий случай учтем и значение 3
             * 2)факультет, возможные значения 1,3,5,11
             * 3)курс - от 1 до 5
             */
            //массив ид факультетов
            var facultiyIds = new[] { 1, 3, 5, 11 };
            var groups = new List<Group>();//резульирующий массив
            var loader = new RepeatableObjectWebLoader(2, 40);
            //перебираем форму обучения
            for (int forma = 1; forma <= 4; forma++) {
                //цикл по факультетам
                for (int i = 0; i < facultiyIds.Length; i++) {
                    for (int kurs = 1; kurs <= 5; kurs++) {
                        string postData = String.Format(HtmlNodeParsers.GroupSelectionPostDataTemplate, forma, facultiyIds[i], kurs);

                        try {
                            var res = loader.Load<Group>(postData).Select(g =>
                           {
                               g.id_Fak = facultiyIds[i];
                               g.id_Forma = forma;
                               g.Kurs = kurs;
                               return g;
                           });
                            groups.AddRange(res);
                        } catch {
                            continue;
                        }
                    }
                }
            }
            SaveGroupsChanges(groups);
        }
        private void SaveGroupsChanges(IEnumerable<Group> groups)
        {
            _database.UpdateGroups(groups);
        }
    }
}
