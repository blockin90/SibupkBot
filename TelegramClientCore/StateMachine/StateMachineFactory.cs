using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Telegram.Bot.Types;
using TelegramClientCore.BotDatabase;
using TelegramClientCore.BotServices;
using TelegramClientCore.StateMachine.States;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace TelegramClientCore.StateMachine
{
    internal class StateMachineFactory
    {
        private ConcurrentDictionary<long, StateMachineContext> connectedUsers = new ConcurrentDictionary<long, StateMachineContext>();
        private readonly IActorResolver actorResolver;

        public StateMachineFactory(IActorResolver actorResolver)
        {
            this.actorResolver = actorResolver;
        }

        /// <summary>
        /// Получение контекста для заданного чата
        /// </summary>
        public StateMachineContext GetContext(ChatId chatId)
        {
            try {
                if (connectedUsers.TryGetValue(chatId.Identifier, out StateMachineContext machineContext)) {//пользователь уже подключен
                    return machineContext;
                }
                machineContext = CreateStateMachineContext(chatId);
                connectedUsers.TryAdd(chatId.Identifier, machineContext);
                return machineContext;
            } catch (Exception ex) {
                MyTrace.WriteLine(ex.GetFullMessage());
                throw;
            }
        }
        /// <summary>
        /// Создание нового контекста для чата в состоянии InitialState для пользователей, 
        /// подключенных через команду /start. Если чат с таким ChatId уже существовал, он будет перезаписан.
        /// </summary>
        public StateMachineContext GetEmptyContext(ChatId chatId)
        {
            try {
                //т.к. событие удаления чата никак не обрабатывается телеграмм апи, может возникнуть ситуация,
                //в которой пользователь уже подключен. Поэтому делаем попытку удалить пользователя с таким ChatId:
                connectedUsers.TryRemove(chatId.Identifier, out StateMachineContext context);
                //после чего создаем "чистого" пользователя и перезатираем информацию о старом, если она была
                context = new StateMachineContext(chatId);
                context.SendMessageAsync("Добро пожаловать!"+Environment.NewLine +
                    "Перед началом работы рекомендуется ознакомиться со справкой с помощью /help, а также списком возможных настроек /settings");
                context.SaveCurrentState();
                connectedUsers.TryAdd(chatId.Identifier, context);
                return context;
            } catch (Exception ex) {
                MyTrace.WriteLine(ex.GetFullMessage());
                throw;
            }
        }

        /// <summary>
        /// Создание нового контекста для чата. Создание производится в 2 этапа: сначала попытка найти
        /// в локальной БД информацию о прежнем подключении, если найдено, восстанавливает данный контекст,
        /// иначае возвращает новый
        /// </summary>
        /// <param name="chatId">идентификатор чата пользователя</param>
        /// <returns>контекст</returns>
        private StateMachineContext CreateStateMachineContext(ChatId chatId)
        {
            StateMachineContext context = new StateMachineContext(chatId, null);
            string stateName = null;
            string extraData = null;
            State state;
            //пробуем получить запись в БД для данного ид чата
            //если найдено-будем пробовать состояние исходя их сохраненной информации
            if (BotDbContext.Instance.TryToGetChatStateRecord(chatId, ref stateName, ref extraData) == false
                || TryToRestoreState(stateName, extraData, context, out state) == false) {
                //не удалось найти состояние, инциаилизируем как InitialState
                //в противном случае, state содержит восстановленное состояние
                //если не найдено, создаем новый
                state = new InitialState(context);
            }
            context.CurrentState = state;
            return context;
        }


        /// <summary>
        /// Попытка восстановить состояние из записи БД
        /// </summary>
        /// <param name="context">контекст, для которого создается состояние</param>
        /// <param name="state">полученное состояние</param>
        /// <returns>true - если успешно и state содержит ненулевое значение, иначе false</returns>
        private bool TryToRestoreState(string stateName, string extraData, StateMachineContext context, out State state)
        {
            state = null;
            //ExtraData должна хранить ФИО преподавателя или ShortName группы
            //пробуем по этим данным восстановить объект и добавить его как параметр в контекст
            //если попытка восстановления целого объекта провалена - возвращаем false
            if (string.IsNullOrEmpty(extraData) == false && actorResolver.TryToGetActor(extraData, out Actor actor)) {
                //если попали сюда, значит восстановили роль пользователя - добавляем ее как параметр в контекст
                try {
                    if (actor is Group) {
                        context.Parameters["Group"] = actor;
                    } else if (actor is Teacher) {
                        context.Parameters["Teacher"] = actor;
                    } else {  //wtf???
                        return false;
                    }
                    //создаем состояние с заданным именем
                    state = Activator.CreateInstance(Type.GetType(stateName), context) as State;
                } catch (Exception e) {
                    MyTrace.WriteLine(e.GetFullMessage());
                    return false;
                }
                return true;
            }
            //восстанавливаем состояние без ExtraData
            return TryToRestoreDefaultState(stateName, context, out state);
        }


        /// <summary>
        /// Попытка восстановить произвольное состояние из записи БД без ExtraData
        /// </summary>
        /// <param name="stateName">название состояния</param>
        /// <param name="context">контекст, для которого создается состояние</param>
        /// <param name="state">полученное состояние</param>
        /// <returns>true - если успешно и state содержит ненулевое значение, иначе false</returns>
        private static bool TryToRestoreDefaultState(string stateName, StateMachineContext context, out State state)
        {
            try {
                state = Activator.CreateInstance(Type.GetType(stateName), context) as State;
            } catch (Exception e) {
                MyTrace.WriteLine(e.GetFullMessage());
                state = null;
                return false;
            }
            return true;
        }
    }
}
