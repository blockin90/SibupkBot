using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.StateMachine.States.SpecialStates
{
    /// <summary>
    /// Обработчик пользовательских команд. 
    /// Не приводит к изменению контекста, не изменяет текущего состояния, клавиатуру и т.п.
    /// </summary>
    class UserCommandState : State
    {
        const string HelpString = "help";
        const string SettingsString = "settings";
        const string ShowTeachersString = "teachers";
        const string ShowFullTimeString = "fulltime";
        const string ShowHolidaysString = "holiday";



        public UserCommandState(StateMachineContext context) : base(context)
        {
        }

        protected override IReplyMarkup ReplyKeyboard => null;

        public override void OnMessageReceive(string message)
        {
            string command = message.Substring(1);
            bool result = false;
            switch (command) {
                case SettingsString:
                    SendSettings();
                    break;
                case ShowTeachersString:
                    result = StateMachineContext.UserConfig.ChangeTeachersVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ преподавателей в расписании");
                    break;
                case ShowFullTimeString:
                    result = StateMachineContext.UserConfig.ChangeFullTimeVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ времени окончания пары");
                    break;
                case ShowHolidaysString:
                    result = StateMachineContext.UserConfig.ChangeHolidaysVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ выходных");
                    break;
                case HelpString:
                    SendHelp(); 
                    break;
                default:
                    StateMachineContext.SendMessageAsync("Недопустимая команда");
                    break;
            }
        }

        private void SendHelp()
        {
            StateMachineContext.SendMessageAsync(ConfigStates.ConfigState.HelpMessage, parseMode : Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        private void SendSettings()
        {
            var commands = new[] {
                $"/{ShowTeachersString} - вкл/выкл показ преподавателей в расписании",
                $"/{ShowFullTimeString} - вкл/выкл время окончания пары, например 8:30-9:50",
                $"/{ShowHolidaysString} - вкл/выкл показ выходных в расписании",
                $"/{HelpString} - получение справки по работе бота",

           };
            
            StateMachineContext.SendMessageAsync( "Выберите команду из списка (можно \"кликом\")" 
                + Environment.NewLine 
                +  String.Join(Environment.NewLine, commands));
        }

        public override void SendStandardMessage()
        {
            throw new NotImplementedException();
        }
    }
}
