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
    public class MessageSender : IMessageSender
    {
        private TelegramBotClient Client { get; set; }

        /// <summary>
        /// Очередь сообщений для отправки
        /// </summary>
        private ConcurrentQueue<Tuple<ChatId, string, IReplyMarkup, ParseMode>> Messages { get; } =
            new ConcurrentQueue<Tuple<ChatId, string, IReplyMarkup, ParseMode>>();

        /// <summary>
        /// Семафор для управления очередью отправки. Максимальный размер очереди ограничиваем 10000 записей
        /// </summary>
        private Semaphore Semaphore { get; } = new Semaphore(0, 10000);

        /// <summary>
        /// Задача, ответственная за обработку сообщений
        /// </summary>
        private Task MessageReaderTask { get; set; }

        public MessageSender(TelegramBotClient client)
        {
            Client = client;
            MessageReaderTask = Task.Run(() => ProcessMessages());
        }

        /// <summary>
        /// Бесконечный цикл обслуживания очереди сообщений
        /// </summary>
        private void ProcessMessages()
        {
            while (true) {
                Semaphore.WaitOne();
                //todo: проверить, что этому клиенту сообщение было отправлено более секунды назад

                Messages.TryDequeue(out Tuple<ChatId, string, IReplyMarkup, ParseMode> message);
                //т.к. действует ограничение 30 сообщений в секунду разным пользователям, вводим искусственную задержку:
                Thread.Sleep(100);//берем с запасом, 10 пользователей в секунду
                //отправляем сообщение
                
                Client.SendTextMessageAsync(message.Item1, message.Item2, replyMarkup: message.Item3, parseMode: message.Item4);
            }
        }

        /// <summary>
        /// Добавление сообщения в очередь
        /// </summary>
        private void AddMessageToQueue(ChatId chatId, string message, IReplyMarkup replyKeyboard, ParseMode parseMode)
        {
            Messages.Enqueue(Tuple.Create(chatId, message, replyKeyboard, parseMode));
            Semaphore.Release();
        }

        private void AddMessageToQueue(Tuple<ChatId, string, IReplyMarkup, ParseMode> message)
        {
            Messages.Enqueue(message);
            Semaphore.Release();
        }

        public void SendAsync(ChatId chatId, string message, IReplyMarkup replyKeyboard = null, ParseMode parseMode = ParseMode.Default)
        {
            AddMessageToQueue(chatId, message, replyKeyboard, parseMode);
        }
        public void SendToAll(string message)
        {
            //перебрать всех пользователей, сформировать очередь сообщений для каждого из них
            var ids = BotDatabase.BotDbContext.Instance.Users.Select(u => u.ChatId).ToArray();
            //todo: выбрать только тех, у кого оформлена подписка
            foreach (var id in ids) {
                ChatId chatId = new ChatId(id);
                SendAsync(chatId, message);
            }
        }
    }
}
