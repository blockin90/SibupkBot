using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramClientCore.BotCache;
using UpkModel;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.StudentStates
{
    class StudentShowScheduleState : ShowScheduleState
    {
        private Group Group
        {
            get
            {
                return StateMachineContext.Parameters["Group"] as Group;
            }
        }
        
        public StudentShowScheduleState(StateMachineContext context, DateTime first, DateTime last)
            : base(context, first, last)
        {
        }

        protected override State GetPreviousState()
        {
            return new InitialState(StateMachineContext);
        }

        protected override IEnumerable<WorkDay> GetSchedule(DateTime from,DateTime to)
        {
            return GroupScheduleCache.Instance.GetWorkDays(Group, from, to);
        }

        protected override string GetMessageHeader()
        {
            return $"Расписание для группы {Group.ShortName}";
        }


        protected override string BuildLessonString(Lesson lesson)
        {
            return new LessonBuilder(StateMachineContext, lesson)
                .AddTime()
                .AddAuditory()
                .AddDiscipline()
                .AddTeacher()
                .AddNewLine()
                .ToString();
        }

        protected override SelectDateState GetDateSelectionState()
        {
            return new StudentSelectDates(StateMachineContext);
        }
    }
}
