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
//        /// ��������� ���������� ��� ������� �� �������� �������� ������� startDate
//        /// </summary>
//        private List<WorkDay> GenerateDaysRange(DateTime startDate, int count)
//        {
//            return Enumerable.Range(0, count)
//                .Select(i => new WorkDay() { Date = startDate.AddDays(i) })
//                .ToList();
//        }
//        /// <summary>
//        /// ��������� ������������� ���������� ��� ������� �� �������� �������� ������� startDate
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
//        /// ��������� ������� �������� ���������� ��� ������ �� 
//        /// </summary>
//        [TestMethod]
//        public void TryToGetWorkDays_EmptyTest()
//        {
//            using (var context = new CacheDbContext())
//            using (var service = new CacheDbService(context)) {
//                var expectedResult = false;
//                IEnumerable<WorkDay> expectedWorkDays = null;
//                //����� ������ � ���� �� �������������, �.�. �� �����
//                var actualResult = service.TryToGetStudentWorkDays(4, DateTime.Today, out IEnumerable<WorkDay> actualWorkDays);
//                Assert.AreEqual(expectedResult, actualResult);
//                Assert.AreEqual(expectedWorkDays, actualWorkDays);  //��������� �� ��������� ������, �.�. ��������� �������� - null
//            }
//        }
//        /// <summary>
//        /// ������� �������� ���������� ��� ������������� ������ � ��� �����, ������� ���� � ��
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
//                    //������� �������� ���������� ��� ������������� ������
//                    var actualResult = service.TryToGetStudentWorkDays(50, DateTime.Today, out IEnumerable<WorkDay> actualWorkDays);
//                    Assert.AreEqual(false, actualResult);
//                    Assert.AreEqual(null, actualWorkDays);  //��������� �� ��������� ������, �.�. ��������� �������� - null
//                    //��� ������������ ����� ��������� ����������� ������� ������������ ����
//                    actualResult = service.TryToGetStudentWorkDays(10, startDate, out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(new DateTime(2018, 9, 10), actualWorkDays.Max(wd => wd.Date).Date);
//                    //��������� ��� ������ ������
//                    actualResult = service.TryToGetStudentWorkDays(20, new DateTime(2018, 9, 12), out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(new DateTime(2018, 9, 12), actualWorkDays.Max(wd => wd.Date).Date);
//                }
//            }
//        }
//        /// <summary>
//        /// ���� ������, ����� ����������� ���������� �������� �������.
//        /// ������������� ��������:
//        /// 1) ����, �� ������� ��������� DateTo ������ CachedGroup �� �������� ����������
//        /// 2) ������������� ����������,���������� ����
//        /// ���� �������� �������� ���������� ����� �� DateTo ��� 
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
//                    //������������� ���� ������ ���, �� ������� ���� ���������� 
//                    actualResult = service.TryToGetStudentWorkDays(20, new DateTime(2018, 9, 10), out actualWorkDays);
//                    Assert.AreEqual(true, actualResult);
//                    Assert.AreEqual(0, actualWorkDays.Count());
//                }
//            }
//        }
//        #endregion

//        #region UpdateStudentCache

//        /// <summary>
//        /// 1) ������, ��� ������� ����������� ���, �� ����������, ������ ���� ������� ����� ������ � �����������
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test1()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                using (var service = new CacheDbService(context)) {
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 17), GenerateDaysRange(date, 5));
//                    var group = context.CachedGroups.Find(10);
//                    Assert.AreNotEqual(null, group);//���� ������,�� ������ ���� �������
//                    Assert.AreEqual(5, group.CachedGroupWorkDays.Count); //��� ������ ������ ���� ��������� 5 ����
//                    Assert.AreEqual(null, context.CachedGroups.FirstOrDefault(g => g.CachedGroupId != 10)); //������� �������� �������� � �������
//                    Assert.AreEqual(0, context.CachedGroupWorkDays.Count(wd => wd.CachedGroupId != 10));//������� �������� �������� � ����������
//                    //
//                    Assert.IsTrue(group.CachedTo != group.CachedGroupWorkDays.Max(wd => wd.Date).Date);
//                    //
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 17), GenerateDaysRange(date, 8));
//                    Assert.AreEqual(new DateTime(2018, 9, 17), group.CachedTo);
//                }
//            }
//        }
//        /// <summary>
//        /// 2) ������ ����������, �� ��� ��� ��� ������, ���� ������ ��� �� ���������
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test2()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                //��������� ������ � "������������" �����������
//                context.CachedGroups.Add(new CachedGroup { CachedGroupId = 10, CachedTo = date.AddDays(-2) });
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    var days = GenerateDaysRange(date, 5);
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 14), days);
//                    var group = context.CachedGroups.Find(10);
//                    Assert.AreNotEqual(null, group);//���� ������,�� ������ ���� �������
//                    Assert.AreEqual(5, group.CachedGroupWorkDays.Count); //��� ������ ������ ���� ��������� 5 ����
//                    Assert.AreEqual(new DateTime(2018, 9, 14), group.CachedTo);
//                }
//            }
//        }
//        /// <summary>
//        /// 3) ������ ����������, ���������� �� ��� ����, ����������� ������ �������� ������������ �� ����� � ���������
//        /// </summary>
//        [TestMethod]
//        public void UpdateStudentCache_Test3()
//        {
//            DateTime date = new DateTime(2018, 9, 10);
//            using (var context = new CacheDbContext()) {
//                //������� ��������� ������
//                context.CachedGroups.Add(new CachedGroup
//                {
//                    CachedGroupId = 10,
//                    CachedTo = date.AddDays(3), //�� 13.09.18
//                    CachedGroupWorkDays = GenerateCachedDaysRange(date, 3) //12.09.18
//                });
//                context.CachedGroups.Add(new CachedGroup
//                {
//                    CachedGroupId = 11,
//                    CachedTo = date.AddDays(5)
//                });
//                context.SaveChanges();
//                using (var service = new CacheDbService(context)) {
//                    var days = GenerateDaysRange(date.AddDays(1), 4);   //������������ ���������� � 11.9.18 �� 14.9.18
//                    //������ ���������� vs ����� ����������:
//                    //10.09   11.09  12.09   --
//                    //        11.09  12.09   13.09   14.09
//                    //�� ����� ������ ���� ��������� ������ 14.09, �.�. �������, ��� ������ �� ����� �� ����������
//                    //������� �� ������ �������� CachedTo ��� ������ �� 13.09 ������ ���� ���������
//                    service.UpdateStudentCache(10, new DateTime(2018, 9, 19), days);
//                    var group = context.CachedGroups.Find(10);
//                    var actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    var expectedDates = Enumerable.Range(0, 3).
//                        Select(num => date.AddDays(num))
//                        .Union(new[] { new DateTime(2018, 9, 14) })
//                        .ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //������� �������� �������� �� ������ ������, ����� ���������� ���� ������ ���� ����� 4:
//                    Assert.AreEqual(4, context.CachedGroupWorkDays.Count());
//                }
//            }
//        }

//        #endregion

//        /// <summary>
//        /// �������� ���������� �������
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
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate, 4) //���� � 10.09 �� 13.09
//                };
//                context.Add(group);
//                group = new CachedGroup()
//                {
//                    CachedGroupId = 11,
//                    CachedTo = startDate.AddDays(6),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate.AddDays(2), 4) //���� � 12.09 �� 15.09
//                };
//                context.Add(group);
//                group = new CachedGroup()
//                {
//                    CachedGroupId = 12,
//                    CachedTo = startDate.AddDays(7),
//                    CachedGroupWorkDays = GenerateCachedDaysRange(startDate.AddDays(4), 3) //���� � 14.09 �� 16.09
//                };
//                context.Add(group);
//                context.SaveChanges();
//                #endregion
//                using (var service = new CacheDbService(context)) {
//                    //������� ���������� �� 12.09, �� ������� ��� �����
//                    DateTime actualDate = new DateTime(2018, 9, 12);
//                    service.RemoveOutOfDateRecords(actualDate);
//                    //groupId=10, ���� � 10.09 �� 13.09: �������� 12.09, 13.09
//                    group = context.CachedGroups.Find(10);
//                    var actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    var expectedDates = Enumerable.Range(0, 2).Select(i => actualDate.AddDays(i)).ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //groupId=11, ���� � 12.09 �� 15.09: �������� 12.09, 13.09, 14.09, 15.09
//                    group = context.CachedGroups.Find(11);
//                    actualDates = group.CachedGroupWorkDays.Select(wd => wd.Date).ToArray();
//                    expectedDates = Enumerable.Range(0, 4).Select(i => actualDate.AddDays(i)).ToArray();
//                    CollectionAssert.AreEquivalent(expectedDates, actualDates);
//                    //groupId=12, ���� � 14.09 �� 16.09: �������� ������ 14.09, 15.09, 16.09
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

