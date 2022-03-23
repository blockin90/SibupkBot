using System;
using System.Collections.Generic;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// Один день расписания
    /// </summary>
    public class WorkDay
    {
        public DateTime Date { get; set; }
        public List<Lesson> Lessons { get; set; }
        public WorkDay(DateTime date)
        {
            Date = date;
            Lessons = new List<Lesson>();
        }
        public WorkDay()
        {

        }

        public override bool Equals(object obj)
        {
            WorkDay other = obj as WorkDay;
            if(other == null) {
                return false;
            }
            var equals = Date.Equals(other.Date);
            equals &= Lessons.Count == other.Lessons.Count;
            if (equals) {
                for (int i = 0; i < Lessons.Count && equals; i++) {
                    equals &= Lessons[i].Equals(other.Lessons[i]);
                }
            }
            return equals;
        }
    }
    public class StudentWorkDay : WorkDay
    {
        public StudentWorkDay(DateTime date)
            :base(date)
        { 
        }
        public StudentWorkDay()
        {

        }
    }
}
