using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramClientCore.BotServices;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace TelegramClientCore.StateMachine.States
{
    internal class InitialState : State
    {
        public const string ConfigString = "Настройки";
        private IActorResolver _actorResolver;

        private static IReplyMarkup _replyKeyboard = MakeReplyKeyboard(new[]
        {
            ConfigString
        });

        public override bool IsNeedToPreserve => true;

        /// <summary>
        /// список доступных Actor'ов, удовлетворяющих частичному совпадению
        /// </summary>
        private IEnumerable<string> availableActors;

        /// <summary>
        /// Клавиатура по умолчанию для первого запуска и случаев, когда поиск завершился неудачей 
        /// и не найдено ни одного варианта 
        /// </summary>
        private static ReplyKeyboardMarkup _defaultKeyboard = MakeReplyKeyboard(
            new[]
            {
                new []{ ConfigString }
            });

        protected override IReplyMarkup ReplyKeyboard
        {
            get
            {
                if (availableActors == null || availableActors.Count() == 0) {
                    return _defaultKeyboard;
                } else {
                    return MakeReplyKeyboard(availableActors.Union(new[] { ConfigString }).ToArray());
                }
            }
        }

        private string MessageToSend
        {
            get
            {
                string defaultMessage = "Если Вы преподаватель, " +
                            "введите свою фамилию, если студент - группу. " + Environment.NewLine +
                            "Можно указать <b>часть названия</b>(первые буквы), в этом случае будут предложены подходящие варианты" + Environment.NewLine +
                            "Например: <i>Петров</i>, <i>петро</i>, <i>ПИБ-61</i>, <i>пиб 51</i>, <i>ИБ-51.1</i>, <i>иф</i>";
                if (availableActors == null) {  //первое попадание в состояние
                    return "Давайте знакомиться! " + Environment.NewLine + defaultMessage;
                } else if (availableActors.Count() == 0) {  //уже были запросы на поиск
                    return "Ничего не найдено ☹️ Пожалуйста, уточните запрос. " + Environment.NewLine + defaultMessage; 
                } else {
                    return "Не найдено точного совпадения. Выберите один из предложенных вариантов " +
                            "или повторите ввод для повторного поиска";
                }
            }
        }

        public InitialState(StateMachineContext context) : base(context)
        {
            _actorResolver = ServiceProvider.GetService<IActorResolver>();
        }

        public override void SendStandardMessage()
        {
            StateMachineContext.SendMessageAsync(MessageToSend, ReplyKeyboard, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        /// <summary>
        /// Сменить состояние автомата для конкретной роли
        /// </summary>
        /// <param name="actor"></param>
        private void ChangeState(Actor actor)
        {
            if( actor is Group) {
                StateMachineContext.Parameters["Group"] = actor;
                StateMachineContext.ChangeState(new StudentStates.StudentSelectDates(StateMachineContext));
            } else {
                StateMachineContext.Parameters["Teacher"] = actor;
                StateMachineContext.ChangeState(new TeacherStates.TeacherSelectAction(StateMachineContext));
            }
        }

        public override void OnMessageReceive(string message)
        {
            if (message.Trim().Length <= 1) {
                StateMachineContext.SendMessageAsync("Минимальная длина строки для поиска - 2 символа!", ReplyKeyboard);
                return;
            } else if( message == StartCommandString) {
                SendStandardMessage();
            } else if (message == ConfigString) {
                StateMachineContext.ChangeState(new ConfigStates.ConfigState(StateMachineContext));
                return;
            } else if (_actorResolver.TryToGetActor(message, out Actor actor)) {//пробуем найти полное совпадение
                ChangeState(actor);
            } else {
                //если не получилось, ищем частиные совпадения и отправляем их пользователю
                availableActors = _actorResolver.GetSimilarActors(message);
                SendStandardMessage();
            }
        }
    }
}
