using System;
using System.Text;
using UpkModel;
using UpkModel.Database;
using UpkModel.Database.Schedule;

namespace TelegramClientCore.StateMachine
{
    internal class LessonBuilder
    {
        private readonly StateMachineContext machineContext;
        private readonly Lesson lesson;
        private readonly StringBuilder stringBuilder = new StringBuilder(1024);

        public LessonBuilder(StateMachineContext machineContext, Lesson lesson)
        {
            this.machineContext = machineContext;
            this.lesson = lesson;
        }

        private void AddDefaultDelimiter()
        {
            if (stringBuilder.Length != 0) {
                stringBuilder.Append(" — ");
            }
        }
        public LessonBuilder AddTime()
        {
            AddDefaultDelimiter();
            string time = machineContext.UserConfig.FullTimeVisibility ? lesson.TimeInterval : lesson.LessonStartTime;
            stringBuilder.Append($"<code>{time}</code>");
            return this;
        }
        public LessonBuilder AddAuditory()
        {
            AddDefaultDelimiter();
            stringBuilder.Append(lesson.Auditory);
            return this;
        }
        public LessonBuilder AddGroup()
        {
            AddDefaultDelimiter();
            stringBuilder.Append(lesson.Group);
            return this;
        }
        public LessonBuilder AddDiscipline()
        {
            AddDefaultDelimiter();
            string discipline = lesson.Discipline +
                (lesson.LessonType == LessonType.Unknown ? "" : $" ({lesson.LessonType.GetFriendlyName()})");
            stringBuilder.Append(discipline);
            if( lesson.Online) {
                stringBuilder.Append(" Онлайн");
            }

            return this;
        }
        public LessonBuilder AddNewLine()
        {
            stringBuilder.Append(Environment.NewLine);
            return this;
        }
        public LessonBuilder AddTeacher()
        {
            if (machineContext.UserConfig.TeachersVisibility) {
                stringBuilder.Append($" / {lesson.TeacherName}");
            }
            return this;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
