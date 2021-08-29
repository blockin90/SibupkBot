using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using UpkModel;
using UpkModel.Database;

namespace UpkServices.Web
{
    /// <summary>
    /// Класс преобразователей вида HtmlNode -> T, где T тип объекта
    /// </summary>
    public static class HtmlNodeParsers
    {
        #region parameters
        public const string TeacherBaseUrl = @"http://old.sibupk.su/services/shedule_new/index.php?mode=2";
        public const string StudentBaseUrl = @"http://old.sibupk.su/services/shedule_new/index.php?mode=1";

        public const string DateIntervalsPostDataTemplate = @"id_KodKaf={0}&FIO={1}";
        public const string SchedulePostDataTemplate = @"id_KodKaf={0}&FIO={1}&RangeNedel={2}";
        public const string GroupSelectionPostDataTemplate = @"id_Forma={0}&id_Fak={1}&Kurs={2}";
        public const string StudentSchedulePostDataTemplate = @"id_Forma={0}&id_Fak={1}&Kurs={2}&NamePodGrup={3}&RangeNedel={4}";
        #region parsers XPaths
        public const string TeacherXpath = @"//table/tr/td/ul/li/ul/li/a";
        public const string GroupsXpath = @"//option";
        public const string TeacherLessonRowXPath = @"/html[1]/body[1]/table/tr[4]/td/table/tr";
        public const string DateIntervalsXpath = @"//table/tr[3]/td[1]/form/p/select/option";
        public const string DepartmentXPath = @"//table/tr/td/form/p/select/option";
        public const string WorkDayXPath = @"//body//table/tr[3]/td[1]/table/tr/th[1][@colspan = 4]";
        public const string StudentWorkDayXPath = @"//body/table/tr[3]/td[1]/table/tr/th[1][@colspan = 5]";
        #endregion
        #region regular expressions
        private static Regex TeacherRegex { get; } = new Regex(@"id_KodKaf=(\d+)");
        private static Regex GroupRegex { get; } = new Regex(@"^[А-ЯЁа-яё]+\-[^(]+");
        private static Regex TeacherFioRegex { get; } = new Regex(@"([А-ЯЁ][а-яё]+) ([А-ЯЁ][а-яё]*) ([А-ЯЁ][а-яё]*)"); //Фамилия И[мя] О[тчество]
        private static Regex TeacherShortFioRegex { get; } = new Regex(@"[А-ЯЁ][а-яё]+ [А-ЯЁ]\.[А-ЯЁ]\."); //Фамилия И.О.
        private static Regex TimeIntervalRegex { get; } = new Regex(@"(\d{2}\.\d{2}.\d{4}) - (\d{2}\.\d{2}.\d{4})");
        private static Regex WorkDayRegex { get; } = new Regex(@"[А-Я][а-я]+\s+\((\d{2}\.\d{2}\.\d{4})\)"); //регулярка на день недели
        private static Regex LessonNumRegex = new Regex(@"\d ");
        private static Regex LessonNameRegex = new Regex(@"^([\w\-\s:,\.""\(\)]+)\((\w+)\)\s*(Онлайн)?$");//вычленение названия дисциплины и ее типа из строки вида 'Название (тип)'
        private static Regex AuditoryRegex = new Regex(@"^а\.(\d+)\s+УК\s+(\d+) ");//вычленение номера аудитории и номера учебного корпуса из выражений вида а.207 УК 1; а.329 УК 1 ЗАЧЕТ 1.06; а.329 УК 1 с 12 нед.
        #endregion
        #endregion
        public static Department DecodeDepartment(HtmlNode node)
        {
            if (!String.IsNullOrEmpty(node.Attributes["value"].Value)) {
                int id = Int32.Parse(node.Attributes["value"].Value);
                return new Department() { Id = id, Name = node.InnerText };
            }
            return null;
        }
        public static UpkModel.Database.Group DecodeGroup(HtmlNode node)
        {
            //после точки - любой символ кроме скобок и пробелам
            Match m = GroupRegex.Match(node.InnerText);
            if (m.Success) {
                var group = new UpkModel.Database.Group()
                {
                    ShortName = m.Value.TrimEnd(),
                    NamePodGrup = node.InnerText
                };
                return group;
            }
            return null;
        }
        public static Teacher DecodeTeacher(HtmlNode node)
        {
            Match m = TeacherFioRegex.Match(node.InnerText);
            if (m.Success) {
                string F = m.Groups[1].Value;
                string I = m.Groups[2].Value.Substring(0, 1);
                string O = m.Groups[3].Value.Substring(0, 1);
                int idKoKaf = Int32.Parse(TeacherRegex.Match(node.Attributes[0].Value).Groups[1].Value);
                return new Teacher() { DepartmentId = idKoKaf, FIO = node.InnerText, ShortFio = $"{F} {I}.{O}." };
            }
            return null;
        }
        public static Teacher DecodeTeacherFromShortFio(string text)
        {
            var match = TeacherShortFioRegex.Match(text);
            string fio;
            if ( match.Success) {
                fio = match.Value;
            } else {
                fio = text;
            }
            return new Teacher { ShortFio = fio, FIO = fio };
        }
        public static WeekInterval DecodeTimeInterval(HtmlNode node)
        {
            Match m = TimeIntervalRegex.Match(node.InnerText);
            if (m.Success) {
                return new WeekInterval()
                {
                    OriginalString = node.InnerText,
                    Start = DateTime.Parse(m.Groups[1].Value),
                    End = DateTime.Parse(m.Groups[2].Value)
                };
            }
            return null;
        }
        internal static WorkDay DecodeWorkDay(HtmlNode node)
        {
            Match m = WorkDayRegex.Match(node.InnerText);
            if (m.Success) {
                var workDay = new WorkDay() { Date = DateTime.Parse(m.Groups[1].Value) };
                /*далее выбираем все строки расписания, пока не встретим строку с одним или двумя столбцами 
                 * (строки со след. датой или указанием четности недели), либо не дойдем до конца (NextSibling==null)*/
                HtmlNode current = node.ParentNode?.NextSibling;
                while (current != null && current.ChildNodes.Count >= 4) {
                    Lesson l = DecodeLesson(current);
                    l.WorkDay = workDay;
                    workDay.Lessons.Add(l);
                    current = current.NextSibling;
                }
                return workDay;
            }
            return null;
        }
        internal static StudentWorkDay DecodeStudentWorkDay(HtmlNode node)
        {
            Match m = WorkDayRegex.Match(node.InnerText);
            if (m.Success) {
                var workDay = new StudentWorkDay() { Date = DateTime.Parse(m.Groups[1].Value) };
                /*далее выбираем все строки расписания, пока не встретим строку с одним или двумя столбцами 
                 * (строки со след. датой или указанием четности недели), либо не дойдем до конца (NextSibling==null)*/
                HtmlNode current = node.ParentNode?.NextSibling;
                while (current != null && current.ChildNodes.Count >= 4) {
                    Lesson l = DecodeLesson(current);
                    if (l.Discipline == "День самостоятельной работы") {
                        return null;
                    }
                    l.WorkDay = workDay;
                    workDay.Lessons.Add(l);
                    current = current.NextSibling;
                }
                return workDay;
            }
            return null;
        }
        internal static Lesson DecodeLesson(HtmlNode node)
        {
            ParseDiscipline(node.ChildNodes[3].InnerText, out string disciplineName, out LessonType lessonType, out bool online);

            return new Lesson()
            {
                LessonNum = Int32.Parse(LessonNumRegex.Match(node.ChildNodes[1].InnerText).Value.TrimEnd()),
                Discipline = disciplineName.TrimEnd(),
                LessonType = lessonType,
                Group = node.ChildNodes[5].InnerText,
                Auditory = ParseAuditory(node.ChildNodes[7].InnerText),
                Online = online,
                Teacher = node.ChildNodes.Count >9 ? DecodeTeacherFromShortFio( node.ChildNodes[9].InnerText) : null 
            };
        }
        internal static string ParseAuditory(string rawAuditory)
        {
            string auditory = string.Empty;
            Match m = AuditoryRegex.Match(rawAuditory);
            if (m.Success) {
                auditory = $"{m.Groups[1].Value} ({m.Groups[2].Value})";
            } else {
                auditory = rawAuditory;
            }
            return auditory;
        }
        /// <summary>
        /// Определяет по строке из расписания название дисциплины, ее тип
        /// </summary>
        /// <param name="discipline">строка в формате Название (тип)</param>
        /// <param name="disciplineName">полученное название дисциплины</param>
        /// <param name="lessonType">полученный тип занятия</param>
        internal static void ParseDiscipline(string discipline, out string disciplineName, out LessonType lessonType, out bool online)
        {
            var match = LessonNameRegex.Match(discipline);
            online = false;
            if (match.Success) {
                disciplineName = match.Groups[1].Value;
                switch (match.Groups[2].Value) {
                    case "лаб":
                        lessonType = LessonType.LabWork;
                        break;
                    case "лек":
                        lessonType = LessonType.Lecture;
                        break;
                    case "с":
                        lessonType = LessonType.Practical;
                        break;
                    case "конс":
                        lessonType = LessonType.Consultation;
                        break;
                    case "экз":
                        lessonType = LessonType.Exam;
                        break;
                    case "зач":
                        lessonType = LessonType.LabWork;
                        break;
                    default:
                        lessonType = LessonType.Unknown;
                        break;
                }
                if( match.Groups.Count == 4 && match.Groups[3].Value == "Онлайн"){
                    online = true;
                } 
            } else {
                disciplineName = discipline;
                lessonType = LessonType.Unknown;
            }
        }

    }
}
