using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices.Web;

namespace UpkTelegramClientTests.BotCacheDataBase
{
    [TestClass]
    public class HtmlNodeParsersTests
    {
        private string GetPage(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"UpkTelegramClientTests.Resources.{resourceName}.html")) {
                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
        private StudentWorkDay[] GetExpectedWorkDays(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"UpkTelegramClientTests.Expected.{resourceName}.xml")) {
                var xmlSerializer = new XmlSerializer(typeof(StudentWorkDay[]));
                return xmlSerializer.Deserialize(stream) as StudentWorkDay[];
            }
        }

        public void TestStudentsSchedule(string fileName, string nodeXpath, Func<HtmlNode, StudentWorkDay> decoder, StudentWorkDay[] expectedNodes)
        {
            var html = GetPage(fileName);
            var actualNodes = HtmlNodeParsers.Decode(html, decoder, nodeXpath).ToArray();            
            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void StudentsScheduleTest()
        {
            TestStudentsSchedule("StudentEmpty", HtmlNodeParsers.StudentWorkDayXPath, HtmlNodeParsers.DecodeStudentWorkDay, new StudentWorkDay[0]);
            TestStudentsSchedule("Student1", HtmlNodeParsers.StudentWorkDayXPath, HtmlNodeParsers.DecodeStudentWorkDay, GetExpectedWorkDays("Student1"));
            TestStudentsSchedule("Student2", HtmlNodeParsers.StudentWorkDayXPath, HtmlNodeParsers.DecodeStudentWorkDay, GetExpectedWorkDays("Student2"));
        }
    }
}

