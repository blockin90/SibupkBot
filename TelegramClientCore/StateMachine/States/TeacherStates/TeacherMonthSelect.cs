using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database;

namespace TelegramClientCore.StateMachine.States.TeacherStates
{
    class TeacherMonthSelect : State
    {
        public const string CurrentMonthString = "Текущий месяц";
        public const string PreviousMonthString = "Прошлый месяц";
        static IReplyMarkup _replyKeyboard = MakeReplyKeyboard(new[]
            {
                CurrentMonthString,
                PreviousMonthString,
                BackString
             });


        public override bool IsNeedToPreserve => true;

        protected override IReplyMarkup ReplyKeyboard => _replyKeyboard;

        public TeacherMonthSelect(StateMachineContext context)
            : base(context)
        {
        }

        public override void OnMessageReceive(string message)
        {
            int monthNum = 0;
            if (message == BackString) {
                StateMachineContext.ChangeState(new TeacherSelectAction(StateMachineContext));
                return;
            }
            if (message == CurrentMonthString) {
                monthNum = DateTime.Today.Month;
            } else if (message == PreviousMonthString) {
                monthNum = DateTime.Today.Month - 1;
                if (monthNum == 0) {
                    monthNum = 12;
                }
            } else {
                if (!int.TryParse(message, out monthNum) || monthNum <=0 || monthNum > 12 ) {
                    SendStandardMessage();
                    return;
                }
            }
            // к текущему моменту месяц декодирован и содержится в monthNum
            // передаем в новое состояние
            StateMachineContext.ChangeState(new TeacherMonthReport(StateMachineContext, monthNum));
        }

        public override void SendStandardMessage()
        {
            StateMachineContext.SendMessageAsync("Выберите месяц из списка или введите номер", ReplyKeyboard);
        }
    }
}
