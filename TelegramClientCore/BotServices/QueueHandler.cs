using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UpkServices;

namespace TelegramClientCore.BotServices
{
    class QueueHandler
    {
        private TelegramBotClient Client { get; }
        private readonly Thread _workerThread;
        private readonly int _poolingInterval;

        /// <summary>
        /// Очередь сообщений для отправки
        /// </summary>
        private ConcurrentQueue<Tuple<ChatId, string, IReplyMarkup, ParseMode>> Messages { get; } =
            new ConcurrentQueue<Tuple<ChatId, string, IReplyMarkup, ParseMode>>();

        /// <summary>
        /// Семафор для управления очередью отправки. Максимальный размер очереди ограничиваем 10000 записей
        /// </summary>
        private SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(0, 10000);        

        public QueueHandler( TelegramBotClient client, int poolingInterval = 50 )
        {
            Client = client;
            _poolingInterval = poolingInterval;
            _workerThread = new Thread(ProcessMessages);
            _workerThread.Start();
        }

        /// <summary>
        /// Бесконечный цикл обслуживания очереди сообщений
        /// </summary>
        private void ProcessMessages()
        {
            while (true)
            {
                Semaphore.Wait();
                //todo: проверить, что этому клиенту сообщение было отправлено более секунды назад

                Messages.TryDequeue(out Tuple<ChatId, string, IReplyMarkup, ParseMode> message);
                //т.к. действует ограничение 30 сообщений в секунду разным пользователям, вводим искусственную задержку:
                Thread.Sleep(_poolingInterval); 
                try { 
                    var task = Client.SendTextMessageAsync(message.Item1, message.Item2, replyMarkup: message.Item3, parseMode: message.Item4);                
                    task.Wait(TimeSpan.FromSeconds(60));
                } catch (Exception ex){
                    MyTrace.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Добавление сообщения в очередь
        /// </summary>
        public void AddMessageToQueue(ChatId chatId, string message, IReplyMarkup replyKeyboard, ParseMode parseMode)
        {
            Messages.Enqueue(Tuple.Create(chatId, message, replyKeyboard, parseMode));
            Semaphore.Release();
        }
    }
}
