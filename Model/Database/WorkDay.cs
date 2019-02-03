using UpkModel.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace UpkModel.Database
{
    /// <summary>
    /// Один день расписания
    /// </summary>
    public class WorkDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public virtual  List<Lesson> Lessons { get; set; }
        public WorkDay()
        {
            Lessons = new List<Lesson>();
        }
        public WorkDay Copy()
        {
            return this.MemberwiseClone() as WorkDay;
        }
    }

}
