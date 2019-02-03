using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UpkModel;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.StateMachine.States.TeacherStates
{
    /// <summary>
    /// Состояние для показа расписания преподавателя на заданном интервале
    /// </summary>
    class TeacherShowScheduleState : ShowScheduleState
    {
        Teacher Teacher
        {
            get
            {
                return StateMachineContext.Parameters["Teacher"] as Teacher;
            }
        }

        public TeacherShowScheduleState(StateMachineContext context, DateTime first, DateTime last) 
            : base(context, first, last)
        {
        }

        protected override State GetPreviousState()
        {
            return new TeacherSelectAction(StateMachineContext);
        }

        protected override IDataLoader<WorkDay> GetDataLoader()
        {
            TeacherWorkDaysLoaderFactory loaderFactory = new TeacherWorkDaysLoaderFactory(UpkDatabaseContext.Instance, Configs.Instance);
            return loaderFactory.GetLoader(Teacher, firstDate, lastDate);
        }

        protected override string GetMessageHeader()
        {
            return $"Расписание для {Teacher.ShortFio}";
        }
    
        protected override string BuildLessonString(Lesson lesson)
        {
            return new LessonBuilder(StateMachineContext, lesson)
                .AddTime()
                .AddAuditory()
                .AddGroup()
                .AddDiscipline()
                .AddNewLine()
                .ToString();
        }

        protected override SelectDateState GetDateSelectionState()
        {
            return new TeacherSelectDateState(StateMachineContext);
        }
    }
}
