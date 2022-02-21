using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramClientCore.BotDatabase;
using TelegramClientCore.BotServices;
using TelegramClientCore.StateMachine.States;
using TelegramClientCore.StateMachine.States.SpecialStates;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace TelegramClientCore.StateMachine
{
    /// <summary>
    /// Экземпляр бота для подключенного клиента
    /// </summary>
    public class StateMachineContext
    {
        public static long AdminChatId { get; set; }

        private readonly ChatId _chatId;
                
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        public IUserConfigService UserConfig { get; }

        public State CurrentState { get; internal set; }

        public long ChatIdentifier
        {
            get { return _chatId.Identifier;  }
        }

        internal MessageSender MessageSender { get; private set; }

        public StateMachineContext(ChatId chatId, State initialState)
        {
            MessageSender = ServiceProvider.GetService<MessageSender>();
            _chatId = chatId;
            UserConfig = UserConfigsFactory.Instance.GetUserConfigService(ChatIdentifier);
            CurrentState = initialState;
        }

        public StateMachineContext(ChatId chatId)
        {
            MessageSender = ServiceProvider.GetService<MessageSender>();
            _chatId = chatId;
            UserConfig = UserConfigsFactory.Instance.GetUserConfigService(ChatIdentifier);
            CurrentState = new InitialState(this);
        }

        public void SendMessageAsync(string message, IReplyMarkup keyboard = null, ParseMode parseMode = ParseMode.Default)
        {
            MessageSender.SendAsync(_chatId, message, keyboard, parseMode);
        }

        public void ChangeState(State newState)
        {
            CurrentState = newState;
            CurrentState.SendStandardMessage();
            SaveCurrentState();
        }

        public void OnMessageReceive(string message)
        {
            if (message[0] == '/' && message != "/start") {
                State commandState;
                if (_chatId.Identifier == AdminChatId) {
                    commandState = new AdminCommandState(this);
                } else {
                    commandState = new UserCommandState(this);
                }
                commandState.OnMessageReceive(message);
            } else {
                CurrentState.OnMessageReceive(message);
            }
        }

        /// <summary>
        /// Сохранение текущего состояния для возможности восстановления работы в случае сбоя.
        /// </summary>
        public void SaveCurrentState()
        {
            if (CurrentState?.IsNeedToPreserve == true) {
                var stateName = CurrentState.GetType().FullName;
                //получаем информацию, необходимую для восстановления контекста:
                string extraData = String.Empty;
                if (stateName.Contains("Teacher") && Parameters.ContainsKey("Teacher")) {
                    extraData = (Parameters["Teacher"] as Teacher).FIO;
                } else if (stateName.Contains("Student") && Parameters.ContainsKey("Group")) {
                    extraData = (Parameters["Group"] as Group).ShortName;
                }
                BotDbContext.Instance.AddOrUpdateChatStateRecord(
                    ChatIdentifier,
                    stateName,
                    extraData);
            }
        }
    }
}
