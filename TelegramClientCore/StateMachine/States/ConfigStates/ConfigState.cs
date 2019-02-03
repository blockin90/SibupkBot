using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.StateMachine.States.ConfigStates
{
    /// <summary>
    /// Состояние, предлагающее пользователю опции настройки:
    /// Параметры отображения
    /// Отписаться
    /// Обратная связь
    /// Справка
    /// </summary>
    internal class ConfigState : State
    {
        public const string HelpMessage = @"Бот предназначен для просмотра расписания СибУПК. 

Последовательность работы с ботом: 
1) Введите группы или ФИО. Не обязательно указывать полное название, можно использовать первые буквы. В этом случае бот предложит варианты, из которых Вы сможете выбрать подходящий;
2) Укажите день (с помощью клавиатуры бота) или конкретную дату (например, 10.11, 10.11.18, 10.11.2018); 
3) Ознакомьтесь с расписанием 😐, при необходимости выберите другой день, как указано в пункте 2;
4) Если необходимо выбрать другую группу, нажмите ""Назад"", после чего повторите процедуру с шага 1; 

Если Вашей группы (или фамилии, если Вы-преподаватель) нет в списке, напишите об этом через форму обратной связи с главной формы.

Для каждого пункта предусмотрен свой список действий, который описан с помощью клавиатуры бота, а требуемое действие запрашивается в приветственном сообщении этого состояния. В случае некорректного ввода Вы получите сообщение об ошибке. 

Среднее время ответа составляет ~2 секунды. Не следует отправлять повторные сообщения до получения ответа на предыдущие.

Бот имеет некоторый набор параметров. С их списком можно ознакомиться с помощью команды /settings, либо войдя в пункт ""Настройки"" на первом экране.

Для сброса бота в начальное состояние используйте команду /start";

        public const string DisplayConfigString = "Параметры отображения";
        public const string FeedbackString = "Обратная связь";
        public const string HelpString = "Справка";

        private static ReplyKeyboardMarkup replyMarkup  = MakeReplyKeyboard( new[]{
            DisplayConfigString,
            FeedbackString,
            HelpString,
            BackString
        } );

        public override bool IsNeedToPreserve => true;

        public ConfigState(StateMachineContext context) : base(context)
        {
        }

        protected override IReplyMarkup ReplyKeyboard => replyMarkup;

        public override void OnMessageReceive(string message)
        {
            switch (message) {
                case DisplayConfigString:
                    StateMachineContext.ChangeState(new DisplayOptionsState(StateMachineContext));
                    break;
                case FeedbackString:
                    StateMachineContext.ChangeState(new FeedbackState(StateMachineContext));
                    break;
                case HelpString:
                    StateMachineContext.SendMessageAsync(HelpMessage, parseMode : Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                case BackString:
                    StateMachineContext.ChangeState(new InitialState(StateMachineContext));
                    break;
                default:
                    SendStandardMessage();
                    break;
            }
        }
        

        public override void SendStandardMessage()
        {
            string message = $"Выберите одно из нижеприведенных действий. Для быстрого доступа к настройкам можно использовать команды. Со списком команд можно ознакомиться введя /help.{Environment.NewLine}" +
                $"<b>{DisplayConfigString}</b> - настройки отображения расписания{Environment.NewLine}" +
                $"<b>{FeedbackString}</b> - здесь Вы сможете оставить свой отзыв по работе сервиса{Environment.NewLine}" +
                $"<b>{HelpString}</b> - справка по работе с ботом";

            StateMachineContext.SendMessageAsync(message, ReplyKeyboard, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}
