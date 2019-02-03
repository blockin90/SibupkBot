using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.BotServices
{
    interface IMessageSender
    {
        /// <summary>
        /// Отправка сообщения заданному пользователю
        /// </summary>
        /// <param name="chatId">идентификатор чата пользователя</param>
        /// <param name="message">сообщения для отправки</param>
        /// <param name="replyKeyboard">клавиатура, прилагаемая к сообщению</param>
        void SendAsync(ChatId chatId, string message, IReplyMarkup replyKeyboard = null, ParseMode parseMode = ParseMode.Default);
        /// <summary>
        /// Широковещательная рассылка заданного сообщения
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        void SendToAll(string message);
    }
}
