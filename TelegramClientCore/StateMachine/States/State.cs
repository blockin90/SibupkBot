using System;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database.Schedule;

namespace TelegramClientCore.StateMachine.States
{
    public abstract class State
    {
        #region user command constants
        public const string BackString = "Назад";
        public const string StartCommandString = "/start";
        #endregion
        public StateMachineContext StateMachineContext { get; }

        public State(StateMachineContext context)
        {
            StateMachineContext = context;
        }
        #region overrided properties/methods

        /// <summary>
        /// Свойство указывающее, что нахождение пользователя в данном состоянии необходимо сохранить.
        /// Требуется переодпределить данное свойство в производных классах,
        /// чтобы была возможность продолжить работу пользователя с данного состояния
        /// в случае сбоя
        /// </summary>
        public virtual bool IsNeedToPreserve
        {
            get => false;
        }

        /// <summary>
        /// Клавиатура, соответствующая состоянию
        /// </summary>
        protected abstract IReplyMarkup ReplyKeyboard { get; }

        /// <summary>
        /// Обработчик полученного сообщения
        /// </summary>
        /// <param name="message">полученное сообщение</param>
        public abstract void OnMessageReceive(string message);
        /// <summary>
        /// Отправка стандартного для текущего состояния сообщения
        /// </summary>
        public abstract void SendStandardMessage();

        #endregion

        /// <summary>
        /// Создает линейную (вертикальную) клавиатуру на основе переданных команд
        /// </summary>
        protected static ReplyKeyboardMarkup MakeReplyKeyboard(string[] keyboardCommands)
        {
            if (keyboardCommands == null || keyboardCommands.Length == 0) {
                throw new ArgumentException("keyboard musr not be empty!");
            }
            string[][] keyboard = new string[keyboardCommands.Length][];
            for (int i = 0; i < keyboardCommands.Length; i++) {
                keyboard[i] = new string[1];
                keyboard[i][0] = keyboardCommands[i];
            }
            ReplyKeyboardMarkup replyKeyboardMarkup = (ReplyKeyboardMarkup)keyboard;
            replyKeyboardMarkup.ResizeKeyboard = true;
            return replyKeyboardMarkup;

        }
        /// <summary>
        /// Создает двумерный ReplyKeyboardMarkup на основе переданных команд
        /// </summary>
        protected static ReplyKeyboardMarkup MakeReplyKeyboard(string[][] keyboardCommands)
        {
            if (keyboardCommands == null || keyboardCommands.Length == 0) {
                throw new ArgumentException("keyboard musr not be empty!");
            }
            ReplyKeyboardMarkup replyKeyboardMarkup = (ReplyKeyboardMarkup)keyboardCommands;
            replyKeyboardMarkup.ResizeKeyboard = true;
            return replyKeyboardMarkup;
        }
    }
}