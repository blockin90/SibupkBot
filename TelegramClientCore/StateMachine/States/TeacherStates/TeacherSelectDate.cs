using System;
using Telegram.Bot.Types.ReplyMarkups;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.TeacherStates
{
    internal class TeacherSelectDateState : SelectDateState
    {
        public override bool IsNeedToPreserve => true;

        public TeacherSelectDateState(StateMachineContext context) : base(context)
        {
        }

        protected override State GetPreviousState()
        {
            return new TeacherSelectAction(StateMachineContext);
        }

        protected override ShowScheduleState GetNextState(DateTime date1, DateTime date2)
        {
            return new TeacherShowScheduleState(StateMachineContext, date1, date2);
        }
    }
}