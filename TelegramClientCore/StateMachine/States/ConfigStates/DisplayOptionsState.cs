using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.StateMachine.States.ConfigStates
{
    class DisplayOptionsState : State
    {
        public const string EnableDisplayingTeacherString = "Включить показ преподавателей";
        public const string DisableDisplayingTeacherString = "Отключить показ преподавателей";
        public const string EnableDisplayingEndTimeString = "Включить время конца пары";
        public const string DisableDisplayingEndTimeString = "Отключить время конца пары";
        public const string EnableDisplayingHolidays = "Включить показ выходных";
        public const string DisableDisplayingHolidays = "Отключить показ выходных";

        protected override IReplyMarkup ReplyKeyboard => MakeReplyKeyboard(new[] {
                $"{(StateMachineContext.UserConfig.TeachersVisibility? DisableDisplayingTeacherString :EnableDisplayingTeacherString )}",
                $"{(StateMachineContext.UserConfig.FullTimeVisibility? DisableDisplayingEndTimeString :EnableDisplayingEndTimeString )}",
                $"{(StateMachineContext.UserConfig.HolidaysVisibility? DisableDisplayingHolidays :EnableDisplayingHolidays )}",
                BackString
           });

        public DisplayOptionsState(StateMachineContext context) : base(context)
        {
        }

        public override void OnMessageReceive(string message)
        {
            bool result = false;
            switch (message) {
                case EnableDisplayingTeacherString:
                case DisableDisplayingTeacherString:
                    result = StateMachineContext.UserConfig.ChangeTeachersVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ преподавателей в расписании");
                    break;
                case EnableDisplayingEndTimeString:
                case DisableDisplayingEndTimeString:
                    result = StateMachineContext.UserConfig.ChangeFullTimeVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ времени окончания пары");
                    break;
                case EnableDisplayingHolidays:
                case DisableDisplayingHolidays:
                    result = StateMachineContext.UserConfig.ChangeHolidaysVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ выходных");
                    break;
                case BackString:
                    StateMachineContext.ChangeState(new ConfigState(StateMachineContext));
                    return;
                default:
                    StateMachineContext.SendMessageAsync("Недопустимая команда");
                    break;
            }
            SendStandardMessage();
        }

        public override void SendStandardMessage()
        {
            StateMachineContext.SendMessageAsync("Выберите одну из нижеприведенных опций. Для быстрого доступа к настройкам можно использовать команды. Со списком команд можно ознакомиться введя /help.", ReplyKeyboard);
        }
    }
}
