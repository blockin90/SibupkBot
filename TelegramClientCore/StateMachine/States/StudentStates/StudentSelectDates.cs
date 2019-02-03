using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.StudentStates
{
    /// <summary>
    /// Состояние выбора диапазона дат для студента
    /// </summary>
    class StudentSelectDates : SelectDateState
    {
        public override bool IsNeedToPreserve => true;

        public StudentSelectDates(StateMachineContext context) : base(context)
        {
        }

        protected override State GetPreviousState()
        {
            return new InitialState(StateMachineContext);
        }

        protected override ShowScheduleState GetNextState(DateTime date1, DateTime date2)
        {
            return new StudentShowScheduleState(StateMachineContext, date1, date2);
        }
    }
}
