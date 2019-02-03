using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.StateMachine.States.ConfigStates
{
    class FeedbackState : State
    {
        static ReplyKeyboardMarkup replyMarkup  = MakeReplyKeyboard( new[]{ BackString} );
        public FeedbackState(StateMachineContext context) 
            : base(context)
        {
        }

        protected override IReplyMarkup ReplyKeyboard => replyMarkup;

        public override void OnMessageReceive(string message)
        {
            //сообщение сохраняется в Program.cs в методе WriteUserActionAsync
            //поэтому в любом случае возвращаемся в InitialState
            StateMachineContext.ChangeState(new ConfigState(StateMachineContext));
        }

        public override void SendStandardMessage()
        {
            StateMachineContext.SendMessageAsync("Оставьте свои пожелания, отзыв по работе боте, для этого просто введите сообщение и нажмите \"Отправить\"", ReplyKeyboard);
        }
    }
}
