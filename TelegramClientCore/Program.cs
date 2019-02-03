
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramClientCore.BotDatabase;
using TelegramClientCore.BotServices;
using TelegramClientCore.StateMachine;
using UpkModel.Database;
using UpkServices;
using User = TelegramClientCore.BotDatabase.User;

namespace TelegramClientCore
{
    internal class Program
    {
        private static TelegramBotClient Client { get; set; }
        private static StateMachineFactory StateMachineFactory { get; set; }
        private static SiteUpdatesNotificationSender SiteUpdatesNotificationSender { get; set; }
        /// <summary>
        /// Настройка среды выполнения: задание трассировщика по умолчанию (вывод в файл), 
        /// включение поддержки русского языка
        /// </summary>
        private static void InitializeRuntime()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#if DEBUG
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
#endif
            Trace.Listeners.Add(new TextWriterTraceListener(new System.IO.StreamWriter("errors.txt",true)));
            Trace.AutoFlush = true;

            //регистрируем поддержку кодировки win-1251
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //задаем CultureInfo для преобразований дат при работе на иностранных серверах
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("ru-RU");
        }

        private static IActorResolver GetActorResolver()
        {
            Console.WriteLine("Loading groups...");
            var groups = new GroupsFactory(UpkDatabaseContext.Instance, Configs.Instance).GetGroups();
            Console.WriteLine("Loading teachers list...");
            var teachers = (new UpkServices.TeachersFactory(UpkDatabaseContext.Instance, Configs.Instance)).GetTeachers();
            return new ActorByNameResolver(teachers, groups);
        }

        /// <summary>
        /// Создание, запуск бота, задание обработчиков основных событий (сообщение, ошибка)
        /// </summary>
        private static void InitializeBot()
        {
            var token = ConfigurationManager.AppSettings.Get("BotToken");
            if( String.IsNullOrEmpty(token))
            {
                throw new ConfigurationErrorsException("App.config must contain valid value for BotToken key in appSettings section");
            }
            var proxyAddress = ConfigurationManager.AppSettings.Get("ProxyAddress");
            if (proxyAddress == null) {
                Client = new TelegramBotClient(token);
            } else {
                Client = new TelegramBotClient(token, new WebProxy(proxyAddress));
            }
            long adminId = 0;
            long.TryParse( ConfigurationManager.AppSettings.Get("AdminId"), out adminId);
            StateMachineContext.AdminChatId = adminId;

            Client.OnMessage += Client_OnMessage;
            Client.OnReceiveError += Client_OnReceiveError;
            Client.OnReceiveGeneralError += Client_OnReceiveGeneralError;
            Client.Timeout = new TimeSpan(0, 0, 2);
            
            var actorResolver = GetActorResolver();
            ISiteUpdateNotificator notificator = new SiteUpdatesChecker(new TimeSpan(1, 0, 0), @"http://www.sibupk.su/student/raspis/");//уведомление об изменениях на сайте

            //регистрируем сервисы
            ServiceProvider.RegisterService(typeof(IMessageSender), new MessageSender(Client)); //сервис отправки 
            ServiceProvider.RegisterService(typeof(IActorResolver), actorResolver);    //сервис определения типа пользователя по его имени
            ServiceProvider.RegisterService(typeof(ISiteUpdateNotificator), notificator);

            notificator.OnSiteUpdate += LogSiteUpdates;
            SiteUpdatesNotificationSender = new SiteUpdatesNotificationSender();
            StateMachineFactory = new StateMachineFactory(actorResolver);
            Client.StartReceiving();
            Console.WriteLine("Server started...");
        }

        private static void LogSiteUpdates(object sender, EventArgs e)
        {
            MyTrace.WriteLine("Сайт обновлен, сброс кэша");
        }

        private static void Main(string[] args)
        {

            InitializeRuntime();
            Console.WriteLine("Starting bot...");
            InitializeBot();
            //ставим задержку, чтобы сервис не закрылся
            TimeSpan sleepInterval = new TimeSpan(2, 0, 0);
            while (Client.IsReceiving) {
                Thread.Sleep(sleepInterval);
            }
        }
        
        #region telegram bot events handling
        private static void Client_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e)
        {
            MyTrace.WriteLine(e.Exception.Message);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MyTrace.WriteLine(e.ExceptionObject.ToString());

        }

        private static void Client_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e)
        {
#if DEBUG
            Console.WriteLine(e.ApiRequestException.Message);
#endif
        }

        private static void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            /* обрабатываем сообщения только от пользоваетелей (отсекаем ботов для исключения 
             * потенциально перегрузки сервиса)
             * Если пользователь сообщение пользователя - команда /start - 
             * это новый пользователь, инициируем его состоянием Initialstate 
             * иначе пробуем найти его среди уже подключенных клиентов, 
             * если не удалось - пробуем восстановить из БД (в случае провала ему будет задан InitialState)
             * .....
             * обрабатываем полученное сообщение полученным контекстом
             */
            if (e.Message.From.IsBot == false) {
                StateMachineContext machineContext = null;
                if (e.Message.Text == "/start") {   //начало нового чата
                    machineContext = StateMachineFactory.GetEmptyContext(e.Message.Chat);
                } else {
                    machineContext = StateMachineFactory.GetContext(e.Message.Chat.Id);
                }
                WriteUserActionAsync(e.Message, machineContext);
                //обрабатываем полученное состояние
                try {
                    machineContext.OnMessageReceive(e.Message.Text);
                } catch (Exception ex) {
                    MyTrace.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        /// <summary>
        /// Сохранение пользовательского запроса в базе данных
        /// </summary>
        /// <param name="message"></param>
        private static void WriteUserActionAsync(Message message, StateMachineContext machineContext)
        {
            try {
                BotDbContext dbInstance = BotDbContext.Instance;
                //если пользователь еще не сохранен в БД, сохраняем
                if (dbInstance.Users.Find(message.Chat.Id) == null) {
                    dbInstance.Users.AddAsync(new User()
                    {
                        ChatId = message.Chat.Id,
                        Username = message.From.Username,
                        LastName = message.From.LastName,
                        FirstName = message.From.FirstName
                    }).ContinueWith(t => { if (t.Exception != null) { Console.WriteLine(t.Exception.Message); } });
                }
                //записываем его текущее состояние и производимое действие
                dbInstance.LogRecords.AddAsync(
                    new LogRecord
                    {
                        ChatId = message.Chat.Id,
                        CurrentState = machineContext?.CurrentState?.ToString(),
                        Message = message.Text,
                        RecordTime = DateInfo.Now
                    }).ContinueWith(t => { if (t.Exception != null) { Console.WriteLine(t.Exception.Message); } });
                dbInstance.SaveChangesAsync().ContinueWith(t => { if (t.Exception != null) { Console.WriteLine(t.Exception.Message); } });
            } catch (Exception e) {
                MyTrace.WriteLine(e.Message);
            }
        }
    }
}