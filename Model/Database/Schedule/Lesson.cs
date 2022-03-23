using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// Учебное занятие
    /// </summary>
    public class Lesson
    {
        public override bool Equals(object obj)
        {
            var other = obj as Lesson;
            if(other == null) {
                return false;
            }
            return other.Online == this.Online &&
                other.Group == this.Group &&
                other.LessonNum == this.LessonNum &&
                other.Auditory == this.Auditory &&
                other.Date == this.Date &&
                other.Discipline == this.Discipline &&
                other.Group == this.Group &&
                other.LessonType == this.LessonType &&
                other.TeacherName == this.TeacherName;


        }
        [Range(1,9)]
        public int LessonNum { get; set; }
        [Required, MaxLength(100)]
        public string Discipline { get; set; }
        [Required, MaxLength(20)]
        public string Group { get; set; }
        [Required, MaxLength(20)]
        public string Auditory { get; set; }
        public bool Online { get; set; }
        public LessonType LessonType { get; set; }
        
        public string TeacherName { get; set; }
        
        public DateTime Date { get; set; }

        [NotMapped]
        public string TimeInterval {
            get {
                switch (LessonNum) {
                    case 1:
                        return "8:30-10:05";
                    case 2:
                        return "10:15-11:50";
                    case 3:
                        return "12:20-13:55";
                    case 4:
                        return "14:05-15:40";
                    case 5:
                        return "15:50-17:25";
                    case 6:
                        return "17:35-19:10";
                    case 7:
                        return "19:20-20:55";
                    case 8:
                        return "21:05-22:40";
                    default:
                        return "очень поздно";
                }
            }
        }
        [NotMapped]
        public string LessonStartTime
        {
            get
            {
                switch (LessonNum) {
                    case 1:
                        return "8:30";
                    case 2:
                        return "10:15";
                    case 3:
                        return "12:15";
                    case 4:
                        return "14:05";
                    case 5:
                        return "15:50";
                    case 6:
                        return "17:35";
                    case 7:
                        return "19:20";
                    case 8:
                        return "21:05"; 
                    default:
                        return "очень поздно";
                }
            }
        }

    }
}
