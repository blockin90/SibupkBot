using System;
using Telegram.Bot.Types.ReplyMarkups;
using UpkServices;

namespace TelegramClientCore.StateMachine.States
{
    class FormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            throw new NotImplementedException();
        }
    }
    internal abstract class SelectDateState : State
    {
        public const string TodayString = "На сегодня";
        public const string TomorrowString = "На завтра";
        public const string WeekString = "На неделю";
        public const string ThreeDaysString = "На 3 дня";

        public bool HasError { get; set; }

        public static ReplyKeyboardMarkup DatesKeyboard = MakeReplyKeyboard(
            new[]
            {
                new []{TodayString, TomorrowString},
                new []{ThreeDaysString, WeekString},
                new []{BackString}
            });

        protected override IReplyMarkup ReplyKeyboard => DatesKeyboard;

        protected abstract State GetPreviousState();
        protected abstract ShowScheduleState GetNextState(DateTime date1, DateTime date2);

        public SelectDateState(StateMachineContext context) : base(context)
        {
        }

        /// <summary>
        /// Попытка декодировать сообщение и определить, позволяет ли оно перейти в новое состояние
        /// </summary>
        /// <param name="dateString"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public bool TryToGetShowScheduleState(string dateString, out State newState)
        {
            newState = null;
            DateTime date = DateInfo.Today;
            if (dateString == TodayString) {                //"На сегодня"
                ;//date = DateInfo.Today;
            } else if (dateString == TomorrowString) {      //"На завтра"
                date = date.AddDays(1);
            } else if (dateString == WeekString) {          //"На неделю"
                newState = GetNextState(date, date.AddDays(6));
                return true;
            } else if (dateString == ThreeDaysString) {    //"На 3 дня"
                newState = GetNextState(date, date.AddDays(2));
                return true;
            } else if (!DateTime.TryParse(dateString, out date)) { //дата в формате дд.ММ, дд.ММ.гг, дд.ММ.гггг
                //дату не получилось декодировать ни одним из способов, просим пользователя ввести ее повторно
                return false;
            }
            // к текущему моменту дата декодирована и содержится в date
            // передаем ее в новое состояние
            newState = GetNextState(date, date);
            return true;
        }

        public override void OnMessageReceive(string message)
        {
            // если команда "Назад" - переход к предшествующему состоянию
            if (message == BackString) {     
                StateMachineContext.ChangeState( GetPreviousState() );
            } else if (TryToGetShowScheduleState(message, out State state)) {//пытаемся распознать дату
                //если успешно, переходим в состояние показа расписания
                StateMachineContext.ChangeState(state);
            } else {
                HasError = true;
                SendStandardMessage();
            }
        }

        public override void SendStandardMessage()
        {
            if (HasError) {
                StateMachineContext.SendMessageAsync($"<b>Некорректная дата!</b>{Environment.NewLine}" +
                    $"Выберите день из меню ниже или введите в формате день.месяц{Environment.NewLine}" +
                    $"Чтобы сменить группу, нажмите \"Назад\"", ReplyKeyboard, Telegram.Bot.Types.Enums.ParseMode.Html);
            } else {
                StateMachineContext.SendMessageAsync("Выберите день из меню или введите в формате день.месяц", ReplyKeyboard);
            }
        }
    }
}
