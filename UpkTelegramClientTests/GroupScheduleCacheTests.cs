//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UpkModel.Database;
//using UpkModel.Database.Schedule;

//namespace UpkTelegramClientTests.BotCache
//{
//    [TestClass]
//    public class CacheDbContextTest
//    {
//        #region test data



//        [TestInitialize]
//        public void Setup()
//        {

//        }


//        /// <summary>
//        /// генерация расписания без занятий на заданный диапазон включая startDate
//        /// </summary>
//        private List<WorkDay> GenerateDaysRange(DateTime startDate, int count)
//        {
//            return Enumerable.Range(0, count)
//                .Select(i => new WorkDay() { Date = startDate.AddDays(i) })
//                .ToList();
//        }
//        /// <summary>
//        /// генерация кэшированного расписания без занятий на заданный диапазон включая startDate
//        /// </summary>
//        private List<WorkDay> GenerateCachedDaysRange(DateTime startDate, int count)
//        {
//            return Enumerable.Range(0, count)
//                .Select(i => new WorkDay() { Date = startDate.AddDays(i) })
//                .ToList();
//        }
//        #endregion

//        #region TryToGetWorkDays
//        /// <summary>
//        /// тестируем попытку получить расписание при пустой БД 
//        /// </summary>
//        [TestMethod]
//        public void TryToGetWorkDays_EmptyTest()
//        {
//            using (var context = new CacheDbContext())
//            using (var service = new CacheDbService(context)) {
//                var expectedResult = false;
//                IEnumerable<WorkDay> expectedWorkDays = null;
//                //номер группы и дата не принципиальны, т.к. БД пуста
//                var actualResult = service.TryToGetStudentWorkDays(4, DateTime.Today, out IEnumerable<WorkDay> actualWorkDays);
//                Assert.AreEqual(expectedResult, actualResult);
//                Assert.AreEqual(expectedWorkDays, actualWorkDays);  //проверяем на равенство ссылок, т.к. ожидаемое значение - null
//            }
//        }
//        /// <summary>
//        /// попытки получить расписание для отсутствующей группы и для групп, которые есть в БД
//        /// </summary>
//        [TestMethod]
//        public void TryToGetWorkDays_InvalidGroupTest()
//        {

//            using (var context = new CacheDbContext()) {
//                DateTime startDate = new DateTime(2018, 9, 10);
//                var group1 = new CachedGroup() { CachedGroupId = 10, CachedTo = startDate, CachedGroupWorkDays = GenerateCachedDaysRange(startDate, 1) };
//                var group2 = new CachedGroup() { CachedGroupId = 20, CachedTo = startDate.AddDays(2), CachedGroupWorkDays = GenerateCachedDaysRange(startDate, 3) };
//                context.Add(group1);
//                context.Add(group2);
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    //попытка получить расписание для отсутствующей группы
//                    var actualResult = service.TryToGetStudentWorkDays(50, DateTime.Today, out IEnumerable<WorkDay> actualWorkDays);
//                    Assert.AreEqual(false, actualResult);
//                    Assert.AreEqual(null, actualWorkDays);  //проверяем на равенство ссылок, т.к. ожидаемое значение - null
//                    //для существующих групп проверяем возможность извлечь максимальные даты
//                    actualResult = service.TryToGetStudentWorkDays(10, startDate, out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(new DateTime(2018, 9, 10), actualWorkDays.Max(wd => wd.Date).Date);
//                    //проверяем для второй группы
//                    actualResult = service.TryToGetStudentWorkDays(20, new DateTime(2018, 9, 12), out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(new DateTime(2018, 9, 12), actualWorkDays.Max(wd => wd.Date).Date);
//                }
//            }
//        }
//        /// <summary>
//        /// тест случая, когда загруженное расписание содержит пробелы.
//        /// рассмотренные ситуации:
//        /// 1) день, на который ссылается DateTo класса CachedGroup не содержит расписания
//        /// 2) запрашивается расписание,содержащее дыры
//        /// было получено получить расписание когда на DateTo нет 
//        /// </summary>
//        [TestMethod]
//        public void TryToGetWorkDays_DateToTest()
//        {
//            using (var context = new CacheDbContext()) {
//                DateTime startDate = new DateTime(2018, 9, 10);
//                var group = new CachedGroup()
//                {
//                    CachedGroupId = 10,
//                    CachedTo = startDate.AddDays(6),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate, 4)
//                };
//                context.Add(group);
//                group = new CachedGroup()
//                {
//                    CachedGroupId = 20,
//                    CachedTo = startDate.AddDays(2),
//                    CachedGroupWorkDays = new List<CachedGroupWorkDay> { new CachedGroupWorkDay { Date = startDate.AddDays(1) } }
//                };
//                context.Add(group);
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    //1):
//                    var actualResult = service.TryToGetStudentWorkDays(10, new DateTime(2018, 9, 16), out IEnumerable<WorkDay> actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(4, actualWorkDays.Count());
//                    //2):
//                    actualResult = service.TryToGetStudentWorkDays(20, new DateTime(2018, 9, 11), out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(1, actualWorkDays.Count());
//                    //запрашиваемая дата меньше той, на которую есть расписание 
//                    actualResult = service.TryToGetStudentWorkDays(20, new DateTime(2018, 9, 10), out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(0, actualWorkDays.Count());
//                }
//            }
//        }
//        #endregion

//        #region UpdateStudentCache

//        /// <summary>
//        /// 1) группы, для которой обновляется кэш, не существует, должна быть создана новая группа с расписанием
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test1()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                using (var service = new CacheDbService(context)) {
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 17), GenerateDaysRange(date, 5));
//                    var group = context.CachedGroups.Find(10);
//                    Assert.AreNotEqual(null, group);//ищем группу,не должна быть нулевой
//                    Assert.AreEqual(5, group.CachedGroupWorkDays.Count); //для группы должно быть добавлено 5 дней
//                    Assert.AreEqual(null, context.CachedGroups.FirstOrDefault(g => g.CachedGroupId != 10)); //никаких побочных эффектов в группах
//                    Assert.AreEqual(0, context.CachedGroupWorkDays.Count(wd => wd.CachedGroupId != 10));//никаких побочных эффектов в расписании
//                    //
//                    Assert.IsTrue(group.CachedTo != group.CachedGroupWorkDays.Max(wd => wd.Date).Date);
//                    //
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 17), GenerateDaysRange(date, 8));
//                    Assert.AreEqual(new DateTime(2018, 9, 17), group.CachedTo);
//                }
//            }
//        }
//        /// <summary>
//        /// 2) группа существует, но для нее нет данных, либо данные уже не актуальны
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test2()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                //добавляем группу с "просроченным" расписанием
//                context.CachedGroups.Add(new CachedGroup { CachedGroupId = 10, CachedTo = date.AddDays(-2) });
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    var days = GenerateDaysRange(date, 5);
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 14), days);
//                    var group = context.CachedGroups.Find(10);
//                    Assert.AreNotEqual(null, group);//ищем группу,не должна быть нулевой
//                    Assert.AreEqual(5, group.CachedGroupWorkDays.Count); //для группы должно быть добавлено 5 дней
//                    Assert.AreEqual(new DateTime(2018, 9, 14), group.CachedTo);
//                }
//            }
//        }
//        /// <summary>
//        /// 3) группа существует, расписание на нее есть, добавляемые данные частично пересекаются по датам с хранимыми
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test3()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                //создаем начальную группу
//                context.CachedGroups.Add(new CachedGroup
//                {
//                    CachedGroupId = 10,
//                    CachedTo = date.AddDays(3), //до 13.09.18
//                    CachedGroupWorkDays = GenerateCachedDaysRange(date, 3) //12.09.18
//                });
//                context.CachedGroups.Add(new CachedGroup
//                {
//                    CachedGroupId = 11,
//                    CachedTo = date.AddDays(5)
//                });
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    var days = GenerateDaysRange(date.AddDays(1), 4);   //генерируется расписание с 11.9.18 до 14.9.18
//                    //старое расписание vs новое расписание:
//                    //10.09   11.09  12.09   --
//                    //        11.09  12.09   13.09   14.09
//                    //по итогу должны быть добавлены только 14.09, т.к. считаем, что данные на сайте не изменялись
//                    //поэтому на основе свойства CachedTo все записи до 13.09 должны быть отброшены
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 19), days);
//                    var group = context.CachedGroups.Find(10);
//                    var actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    var expectedDates = Enumerable.Range(0, 3).
//                        Select(num => date.AddDays(num))
//                        .Union(new[] { new DateTime(2018, 9, 14) })
//                        .ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //никаких побочных эффектов на другие группы, общее количество дней должно быть равно 4:
//                    Assert.AreEqual(4, context.CachedGroupWorkDays.Count());
//                }
//            }
//        }

//        #endregion

//        /// <summary>
//        /// Удаление устаревших записей
//        /// </summary>
//        [TestMethod]
//        public void RemoveOutOfDateRecordsTest()
//        {
//            using (var context = new CacheDbContext()) {
//                DateTime startDate = new DateTime(2018, 9, 10);
//                #region initial data
//                var group = new CachedGroup()
//                {
//                    CachedGroupId = 10,
//                    CachedTo = startDate.AddDays(6),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate, 4) //даты с 10.09 до 13.09
//                };
//                context.Add(group);
//                group = new CachedGroup()
//                {
//                    CachedGroupId = 11,
//                    CachedTo = startDate.AddDays(6),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate.AddDays(2), 4) //даты с 12.09 до 15.09
//                };
//                context.Add(group);
//                group = new CachedGroup()
//                {
//                    CachedGroupId = 12,
//                    CachedTo = startDate.AddDays(7),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate.AddDays(4), 3) //даты с 14.09 до 16.09
//                };
//                context.Add(group);
//                context.SaveChanges();
//                #endregion
//                using (var service = new CacheDbService(context)) {
//                    //удаляем расписание до 12.09, не включая это число
//                    DateTime actualDate = new DateTime(2018, 9, 12);
//                    service.RemoveOutOfDateRecords(actualDate);
//                    //groupId=10, даты с 10.09 до 13.09: остается 12.09, 13.09
//                    group = context.CachedGroups.Find(10);
//                    var actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    var expectedDates = Enumerable.Range(0, 2).Select(i => actualDate.AddDays(i)).ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //groupId=11, даты с 12.09 до 15.09: остается 12.09, 13.09, 14.09, 15.09
//                    group = context.CachedGroups.Find(11);
//                    actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    expectedDates = Enumerable.Range(0, 4).Select(i => actualDate.AddDays(i)).ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //groupId=12, даты с 14.09 до 16.09: остается только 14.09, 15.09, 16.09
//                    group = context.CachedGroups.Find(12);
//                    actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    expectedDates = Enumerable.Range(2, 3).Select(i => actualDate.AddDays(i)).ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                }
//            }
//        }
//        /* using( var context = GetEmptyContext()) {
//             context.UpdateCache(10, new WorkDay[] { new WorkDay()
//             {
//                 Date = DateTime.Today,
//                 Lessons = new List<Lesson>{ new Lesson()
//                 {
//                     Auditory = "23",
//                     Discipline = "123",
//                     Group = "fds",
//                     LessonNum = 1,
//                     LessonType = UpkModel.LessonType.Lecture
//                 }
//             } } });
//             //context.AddRange(new CachedGroup() { CachedTo = DateTime.Today });
//             */
//        //  context.TryToGetWorkDays(20, DateTime.Now, out IEnumerable<WorkDay> ws);
//    }

//}

