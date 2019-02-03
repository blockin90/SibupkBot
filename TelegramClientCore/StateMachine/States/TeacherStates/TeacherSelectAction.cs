using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database;

namespace TelegramClientCore.StateMachine.States.TeacherStates
{
    /// <summary>
    /// Состояние выбора действия - просмотр отчет по нагрузке или расписания
    /// </summary>
    class TeacherSelectAction : State
    {
        static IReplyMarkup _replyKeyboard = MakeReplyKeyboard(
            new[]
            {
                MonthReportString,
                ScheduleString,
                BackString
            });
        public const string MonthReportString = "Отчет по нагрузке";
        public const string ScheduleString = "Расписание";

        protected override IReplyMarkup ReplyKeyboard => _replyKeyboard;

        public override bool IsNeedToPreserve => true;


        public TeacherSelectAction(StateMachineContext context) : base(context)
        {
        }

        public override void OnMessageReceive(string message)
        {
            if(message == BackString) {
                StateMachineContext.ChangeState(new InitialState(StateMachineContext));
            }else if(message == MonthReportString) {
                StateMachineContext.ChangeState(new TeacherMonthSelect(StateMachineContext));
            }else if( message == ScheduleString) {
                StateMachineContext.ChangeState(new TeacherSelectDateState(StateMachineContext));
            } else {
                SendStandardMessage();
            }
        }

        public override void SendStandardMessage()
        {
            StateMachineContext.SendMessageAsync("Выберите действие", ReplyKeyboard);
        }
    }
}
