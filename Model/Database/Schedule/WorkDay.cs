using System;
using System.Collections.Generic;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// Один день расписания
    /// </summary>
    public class WorkDay
    {
        public DateTime Date { get; }
        public List<Lesson> Lessons { get; }
        public WorkDay(DateTime date)
        {
            Date = date;
            Lessons = new List<Lesson>();
        }
        //public WorkDay Copy()
        //{
        //    return this.MemberwiseClone() as WorkDay;
        //}
    }
    public class StudentWorkDay : WorkDay
    {
        public StudentWorkDay(DateTime date)
            :base(date)
        { 
        }
    }
}
