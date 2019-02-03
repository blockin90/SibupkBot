using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.StateMachine.States
{
    internal abstract class ShowScheduleState : State
    {
        protected DateTime firstDate;
        protected DateTime lastDate;

        /* клавиатуру для текущего состояния берем из состояния выбора даты, 
         * чтобы дать возможность пользователю без перехода назад получить новое расписание*/
        protected override IReplyMarkup ReplyKeyboard => SelectDateState.DatesKeyboard;

        public ShowScheduleState(StateMachineContext context, DateTime first, DateTime last)
            : base(context)
        {
            firstDate = first;
            lastDate = last;
        }

        public override void OnMessageReceive(string message)
        {
            if (message == BackString) {
                StateMachineContext.ChangeState(GetPreviousState());
            } else {
                /* если не кнопка назад, то, возможно, дата.
                 * Обрабатываем сообщение состоянием SelectDates, в случае успешного разбора 
                 * оно переводим автомат в возвращенное состояние ShowScheduleState для новой выбранной даты,
                 * иначе переводим автомат в SelectDates до того момента, как пользователь 
                 * введет правильную дату*/
                var dateState = GetDateSelectionState();
                if (dateState.TryToGetShowScheduleState(message, out State newState)) {
                    StateMachineContext.ChangeState(newState);
                } else {
                    dateState.HasError = true;
                    StateMachineContext.ChangeState(dateState);
                }
            }
        }

        public override void SendStandardMessage()
        {
            try {
                var result = GetSchedule().Result;
                if (string.IsNullOrEmpty(result)) {
                    string dateInterval = firstDate.ToString("dd.MM") +
                        (firstDate == lastDate ? "" : " - " + lastDate.ToString("dd.MM"));
                    result = $"<b>{GetMessageHeader()}  на {dateInterval} отсутсвует</b>";
                } else {
                    result = GetMessageHeader() + Environment.NewLine + result;
                }
                StateMachineContext.SendMessageAsync(result, ReplyKeyboard, ParseMode.Html);
            } catch {
                MyTrace.WriteLine($"user id = {StateMachineContext.ChatIdentifier}");
                StateMachineContext.SendMessageAsync("Во время запроса произошла ошибка. Повторите попытку позже.", ReplyKeyboard);
            }
        }

        /// <summary>
        /// Добавляет дни без пар к загруженному расписанию
        /// </summary>
        private IEnumerable<WorkDay> AddHolidays(IEnumerable<WorkDay> workDays)
        {
            int daysCount = (lastDate - firstDate).Days + 1;
            if (workDays.Count() == daysCount) {//последовательность не содержит пробелов
                return workDays;
            }
            WorkDay[] result = new WorkDay[daysCount];
            int position = 0;
            DateTime date = firstDate;
            IEnumerator<WorkDay> wdEnumerator = workDays.GetEnumerator();
            while (position < daysCount) {
                if (wdEnumerator.MoveNext() == false) {
                    break;
                }
                DateTime wdDate = wdEnumerator.Current.Date;
                if (wdDate > date) {
                    FillEmptyDays(result, ref position, date, (wdDate - date).Days);
                }
                result[position++] = wdEnumerator.Current;
                date = wdDate.AddDays(1);
            }
            //сюда попадаем, если перечислитель при вызове Move вернул false, 
            //т.е. коллекция закончилась - добиваем просто датами
            FillEmptyDays(result, ref position, date, daysCount - position);
            return result;
        }

        private void FillEmptyDays(WorkDay[] days, ref int position, DateTime from, int daysCount)
        {
            for (int i = 0; i < daysCount; ++i) {
                days[position++] = new WorkDay() { Date = from.AddDays(i) };
            }
        }

        /// <summary>
        /// Запрос расписания для выбранного диапазона дат
        /// </summary>
        /// <returns>строка, содержащая расписания на указанный период</returns>
        private async Task<string> GetSchedule()
        {
            var result = await GetDataLoader().LoadAsync();
            if (StateMachineContext.UserConfig.HolidaysVisibility) {    //включена опция показа выходных дней?
                result = AddHolidays(result);
            }
            return String.Join($"{Environment.NewLine}{Environment.NewLine}", GetScheduleAsStrings(result));
        }

        /// <summary>
        /// Преобразование последовательности WorkDay в последовательность строк, 
        /// представляющих расписание этих дней
        /// </summary>
        private IEnumerable<string> GetScheduleAsStrings(IEnumerable<WorkDay> workDays)
        {
            foreach (var wd in workDays) {
                if (wd.Lessons.Count == 0) {
                    yield return $"📒 <b> {wd.Date.ToString("dd.MM.yyyy - ddd")} — выходной</b>";
                } else {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"📒 <b>{wd.Date.ToString("dd.MM.yyyy - ddd")}</b>{Environment.NewLine}{Environment.NewLine}");
                    foreach (var lesson in wd.Lessons) {
                        sb.Append("     "); //отступ для строки с названием предмета
                        sb.Append(BuildLessonString(lesson));
                    }
                    yield return sb.ToString();
                }
            }
        }

        /// <summary>
        /// Получение заголовка сообщения, например, для группы - название группы, для преподавателя - ФИО 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetMessageHeader();

        /// <summary>
        /// Получение состояния, отвечающее за выбор дат для данной категории пользователей
        /// </summary>
        protected abstract SelectDateState GetDateSelectionState();
        /// <summary>
        /// Получение состояния, в которое состоится переход по кнопке "Назад"
        /// </summary>
        /// <returns></returns>
        protected abstract State GetPreviousState();
        /// <summary>
        /// Получение загрузчка расписания для данной категории пользователей
        /// </summary>
        protected abstract IDataLoader<WorkDay> GetDataLoader();
        /// <summary>
        /// Форматирование вывода занятий, определение перечня выводимых полей для данной категории пользователей 
        /// </summary>
        /// <param name="lesson">учебное занятие</param>
        /// <returns>строка, передаваемая в качестве ответа пользователю</returns>
        protected abstract string BuildLessonString(Lesson lesson);
    }
}
