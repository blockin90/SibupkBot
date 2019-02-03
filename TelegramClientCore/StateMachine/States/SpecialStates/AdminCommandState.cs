using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.SpecialStates
{
    /// <summary>
    /// Состояние - обработчик команд, не приводящее к изменению состояние контекста
    /// Используется только для обработки команды
    /// могут вызываться только для пользователей, помеченных как администратор бота
    /// </summary>
    class AdminCommandState : State
    {
        const string HelpString = "help";
        const string ShowStatString = "showstat";
        const string ShowUsersString = "showusers";
        const string UpdateActorsString = "updateactors";
        const string SendBroadcastString = "send";
        const string ShowTeachersString = "teachers";
        const string ShowFullTimeString = "fulltime";

        const string ShowHolidaysString = "holiday";
        public AdminCommandState(StateMachineContext context) : base(context)
        {
        }

        protected override IReplyMarkup ReplyKeyboard => null;

        public override void OnMessageReceive(string message)
        {
            string command = message.Substring(1);
            switch (command) {
                case HelpString:
                    SendHelp();
                    break;
                case ShowStatString:
                    SendStats();
                    break;
                case ShowUsersString:
                    SendUsers();
                    break;
                case UpdateActorsString:
                    UpdateActors();
                    break;
                case ShowTeachersString:
                    bool result = StateMachineContext.UserConfig.ChangeTeachersVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ преподавателей в расписании");
                    break;
                case ShowFullTimeString:
                    bool result2 = StateMachineContext.UserConfig.ChangeFullTimeVisibility();
                    StateMachineContext.SendMessageAsync($"{(result2 ? "Включен" : "Отключен")} показ времени окончания пары");
                    break;
                case ShowHolidaysString:
                    result = StateMachineContext.UserConfig.ChangeHolidaysVisibility();
                    StateMachineContext.SendMessageAsync($"{(result ? "Включен" : "Отключен")} показ выходных");
                    break;
                default:
                    if(command.StartsWith("send")) {
                        command = command.Substring(4);
                        SendBroadcastMessage(command);
                    }
                    break;
            }
        }
        private void SendBroadcastMessage(string message)
        {
            StateMachineContext.MessageSender.SendToAll(message);
        }
        private void UpdateActors()
        {
           /* int before = Program.Groups.Count();
            Program.LoadGroups(true);
            int after = Program.Groups.Count();
        */        
            StateMachineContext.SendMessageAsync($"Требуется реализовать полное обновление");
        }

        private void SendHelp()
        {
            var commands = new[] {
                $"/{HelpString} - показывает список доступных команд ",
                $"/{ShowStatString} - показывает количество запросов и уникальных посетителей за последние сутки ",
                $"/{ShowUsersString} - показывает уникальных посетителей и время последнего посещения за сутки ",
                /*$"/{UpdateTeachersString} - актуализирует список преподавателей",
                $"/{UpdateGroupsString} - актуализирует список групп всех факультетов очной формы обучения",*/
                $"/{ShowTeachersString} - вкл/выкл показ преподавателей в расписании",
                $"/{ShowFullTimeString} - вкл/выкл время окончания пары, например 8:30-9:50",
                $"/{ShowHolidaysString} - вкл/выкл показ выходных в расписании",
                $"/{SendBroadcastString} Текст - рассылка широковещательного сообщения всем пользователям"
            };
            StateMachineContext.SendMessageAsync(string.Join(Environment.NewLine, commands));
        }

        private void SendStats()
        {
            var uniqueUsers = BotDatabase.BotDbContext.Instance.LogRecords
                .Where(lr => lr.RecordTime.Date == DateInfo.Today)
                .Select(lr => lr.ChatId)
                .Distinct()
                .Count();
            var recordsCount = BotDatabase.BotDbContext.Instance.LogRecords
                .Where(lr => lr.RecordTime.Date == DateInfo.Today)
                .Count();
            var scheduleCount = BotDatabase.BotDbContext.Instance.LogRecords
                .Where(lr => 
                    lr.RecordTime.Date == DateInfo.Today && 
                    ( lr.CurrentState == "TelegramClientCore.StateMachine.States.StudentStates.StudentShowScheduleState" 
                       || lr.CurrentState == "TelegramClientCore.StateMachine.States.TeacherStates.TeacherShowScheduleState")
                     )
                .Count();
            StateMachineContext.SendMessageAsync($"Статистика за сутки: {Environment.NewLine}" +
                $"Уникальных пользователей: {uniqueUsers}{Environment.NewLine}" +
                $"Сообщений обработано: {recordsCount}{Environment.NewLine}" +
                $"Запросов расписания: {scheduleCount}");
        }
        private void SendUsers()
        {
            var uniqueUserIds = BotDatabase.BotDbContext.Instance.LogRecords
                .Where(lr => lr.RecordTime.Date == DateInfo.Today)
                .Select(lr => lr.ChatId)
                .Distinct();
            //плохой код:
            var users = BotDatabase.BotDbContext.Instance.Users.AsNoTracking().ToArray();
            var result = String.Join(
                Environment.NewLine,
                uniqueUserIds
                .Join(
                    users, 
                    uid => uid, 
                    u => u.ChatId, 
                    (uid, u) => u
                    )
                .ToArray()
                .Select((u,ind) => $"{ind+1}. {u.Username} {u.LastName} {u.FirstName}")
            );
            StateMachineContext.SendMessageAsync(result);
        }
        public override void SendStandardMessage()
        {
            //throw new NotImplementedException();
        }
    }
}
