using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramClientCore.BotServices
{
    /// <summary>
    /// оптравитель сообщений клиентам с учетом ограничений телеграма:
    /// не более одного сообщения в секунду одному пользователю и не более 30 сообщений в секунду разным
    /// </summary>
    public class MessageSender
    {
        QueueHandler _messageHandler;
        QueueHandler _broadcastMessageHandler;

        public MessageSender(TelegramBotClient client)
        {
            _messageHandler = new QueueHandler(client, 50);
            _broadcastMessageHandler = new QueueHandler(client, 500);
        }

        public void SendAsync(ChatId chatId, string message, IReplyMarkup replyKeyboard = null, ParseMode parseMode = ParseMode.Default)
        {
            _messageHandler.AddMessageToQueue(chatId, message, replyKeyboard, parseMode);
        }

        public void SendBroadcastMessage(string message)
        {
            var ids = BotDatabase.BotDbContext.Instance
                .Users
                .Select( record => record.ChatId)
                .ToArray();
            foreach (var id in ids) {
                _broadcastMessageHandler.AddMessageToQueue(new ChatId(id), message, null, ParseMode.Default);
            }
        }

        public void SendScheduleChangingNotification(string message)
        {
            long[] ids;
            lock (BotDatabase.BotDbContext.syncObject) {
                ids = BotDatabase.BotDbContext.Instance
                    .Users
                    .Where(record => !string.IsNullOrEmpty(record.ExtraData))
                    .Select(record => record.ChatId)
                    .ToArray();
            }
            foreach (var id in ids) {
                _broadcastMessageHandler.AddMessageToQueue(new ChatId(id), message, null, ParseMode.Default);
            }
        }
    }
}
