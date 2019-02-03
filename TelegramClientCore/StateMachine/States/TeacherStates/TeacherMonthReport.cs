using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.TeacherStates
{
    class TeacherMonthReport : State
    {
        private readonly int _monthNum;
        static IReplyMarkup _replyKeyboard = MakeReplyKeyboard(new[] { BackString });
        Teacher Teacher
        {
            get
            {
                return StateMachineContext.Parameters["Teacher"] as Teacher;
            }
        }

        protected override IReplyMarkup ReplyKeyboard => _replyKeyboard;

        public TeacherMonthReport(StateMachineContext context, int monthNum)
            : base(context)
        {
            _monthNum = monthNum;
        }

        public override void OnMessageReceive(string message)
        {
            if (message == BackString) {
                StateMachineContext.ChangeState(new TeacherMonthSelect(StateMachineContext));
            }
        }

        public override void SendStandardMessage()
        {
            Task.Run(() =>
            {
                try {
                    var result = GetFormattedResponse();
                    StateMachineContext.SendMessageAsync(result, ReplyKeyboard, ParseMode.Html);
                } catch (Exception e) {
                    StateMachineContext.SendMessageAsync("Во время запроса произошла ошибка. Повторите попытку позже.", ReplyKeyboard);
                    MyTrace.WriteLine(e.Message);
                }
            });
        }

        string GetFormattedResponse()
        {
            int hoursInLesson = 2;
            var queryResult = GetLessons();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"<b>Нагрузка {Teacher.ShortFio} за { DateTimeFormatInfo.CurrentInfo.MonthNames[_monthNum - 1]} (в часах):</b>")
                   .AppendLine($"Лекций:  {hoursInLesson * queryResult.Count(l => l.LessonType == LessonType.Lecture)}")
                   .AppendLine($"Лабор-х: {hoursInLesson * queryResult.Count(l => l.LessonType == LessonType.LabWork)}")
                   .AppendLine($"Практик: {hoursInLesson * queryResult.Count(l => l.LessonType == LessonType.Practical)}")
                   .AppendLine($"Конс-й:  {queryResult.Count(l => l.LessonType == LessonType.Consultation)}");
            if (queryResult.FirstOrDefault(l => l.LessonType == LessonType.Unknown) != null) {
                builder.AppendLine($"<b>Отображена не вся нагрузка. Есть нераспознанные занятия. Подробности см. в версии для ПК.</b>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Загрузка всех занятий за месяц
        /// </summary>
        /// <returns></returns>
        IEnumerable<Lesson> GetLessons()
        {
            var loaderFactory = new TeacherWorkDaysLoaderFactory(UpkDatabaseContext.Instance, Configs.Instance);
            GetDateIntervalFromMonth(_monthNum, out DateTime firstDate, out DateTime lastDate);
            var loader = loaderFactory.GetLoader(Teacher, firstDate, lastDate);
            return loader.Load().SelectMany(wd => wd.Lessons).ToArray();
        }
        //todo: копипаста с вью модели, некруто (
        /// <summary>
        /// На основании месяца генерирует первую и последнюю дату месяца учебного года.
        /// При этом учитывается текущий месяц учебного года. Выбирать данные можно только для прошедших месяцев.
        /// Если будет в феврале будет сделана попытка выбрать данные за ноябрь, то будут выгружены данные за ноябрь прошлого года
        /// </summary>
        /// <param name="monthNum">номер месяца, отсчитываемый с единицы</param>
        /// <param name="firstDate">первая дата месяца</param>
        /// <param name="lastDate">последняя дата месяца</param>
        void GetDateIntervalFromMonth(int monthNum, out DateTime firstDate, out DateTime lastDate)
        {
            int year = DateTime.Today.Year;
            int currentMonth = DateTime.Today.Month;
            if (monthNum > currentMonth) {
                year--;
            }
            firstDate = new DateTime(year, monthNum, 1);
            lastDate = new DateTime(year, monthNum, DateTime.DaysInMonth(year, monthNum));
        }
    }
}
