using SibupkCommonServices.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SibupkServices.Tests.Parsers
{

    //todo: в тестах Invalid* может криво отработать при выполнении теста после ~23:59+, подумать о замене получения текущей даты на сервис
    internal class SiteUpdatesParserTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidDateParserTest()
        {
            var pageContent = Utilites.GetHtmlPage("ScheduleUpdatePage");
            SiteUpdatesParser parser = new SiteUpdatesParser();

            var actualDate = parser.Parse(pageContent);
            var expected = new DateTime(2024, 05, 24);

            Assert.That(actualDate, Is.EqualTo(expected));
        }

        [Test]
        public void InvalidDateValueParserTest()
        {
            var pageContent = Utilites.GetHtmlPage("ScheduleUpdatePage").Replace("24.05.2024", "24.13.2024");
            SiteUpdatesParser parser = new SiteUpdatesParser();

            var actualDate = parser.Parse(pageContent);
            var expected = DateTime.Today;

            Assert.That(actualDate, Is.EqualTo(expected));
        }

        [Test]
        public void MissingDateParserTest()
        {
            var pageContent = Utilites.GetHtmlPage("ScheduleUpdatePage").Replace("Последнее обновление:", string.Empty);
            SiteUpdatesParser parser = new SiteUpdatesParser();

            var actualDate = parser.Parse(pageContent);
            var expected = DateTime.Today;

            Assert.That(actualDate, Is.EqualTo(expected));
        }
    }
}
