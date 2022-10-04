using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database;
using UpkModel.Database.Schedule;
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
                var scheduleTask = Task.Run( ()=>GetSchedule());                
                
                if (scheduleTask.Wait(Settings.ScheduleRequestTimeOut))
                {
                    var result = scheduleTask.Result;
                    if (string.IsNullOrEmpty(result))
                    {
                        string dateInterval = firstDate.ToString("dd.MM") +
                            (firstDate == lastDate ? "" : " - " + lastDate.ToString("dd.MM"));
                        result = $"<b>{GetMessageHeader()}  на {dateInterval} отсутсвует</b>";
                    }
                    else
                    {
                        result = GetMessageHeader() + Environment.NewLine + result;
                    }
                    StateMachineContext.SendMessageAsync(result, ReplyKeyboard, ParseMode.Html);
                }
                else
                {
                    throw new TimeoutException();
                }
            } catch (Exception exception) {
                MyTrace.WriteLine($"user id = {StateMachineContext.ChatIdentifier}, error message: {exception.GetFullMessage()}");
                StateMachineContext.SendMessageAsync("Во время запроса произошла ошибка. Повторите попытку позже.", ReplyKeyboard);
            }
        }

        private void FillEmptyDays(WorkDay[] days, ref int position, DateTime from, int daysCount)
        {
            for (int i = 0; i < daysCount; ++i) {
                days[position++] = new WorkDay( from.AddDays(i));
            }
        }

        /// <summary>
        /// Запрос расписания для выбранного диапазона дат
        /// </summary>
        /// <returns>строка, содержащая расписания на указанный период</returns>
        private async Task<string> GetSchedule()
        {
            var result = await Task.Run( ()=> GetSchedule(firstDate,lastDate));            
            if(result == null) {
                return string.Empty;
            }
            return string.Join($"{Environment.NewLine}{Environment.NewLine}", GetScheduleAsStrings(result));
        }

        /// <summary>
        /// Преобразование последовательности WorkDay в последовательность строк, 
        /// представляющих расписание этих дней
        /// </summary>
        private IEnumerable<string> GetScheduleAsStrings(IEnumerable<WorkDay> workDays)
        {
            workDays = workDays.Where(wd => wd != null);
            foreach (var wd in workDays) {
                if (wd.Lessons.Count == 0) {
                    if (StateMachineContext.UserConfig.HolidaysVisibility) {
                        yield return $"📒 <b> {wd.Date.ToString("dd.MM.yyyy - ddd")} — выходной</b>";
                    } else {
                        continue;
                    }
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
        protected abstract IEnumerable<WorkDay> GetSchedule(DateTime from, DateTime to);
        /// <summary>
        /// Форматирование вывода занятий, определение перечня выводимых полей для данной категории пользователей 
        /// </summary>
        /// <param name="lesson">учебное занятие</param>
        /// <returns>строка, передаваемая в качестве ответа пользователю</returns>
        protected abstract string BuildLessonString(Lesson lesson);
    }
}
