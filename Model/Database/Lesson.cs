using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UpkModel.Database
{
    /// <summary>
    /// Учебное занятие
    /// </summary>
    public class Lesson
    {
        public int Id { get; set; }

        [Range(1,9)]
        public int LessonNum { get; set; }
        [Required, MaxLength(100)]
        public string Discipline { get; set; }
        [Required, MaxLength(20)]
        public string Group { get; set; }
        [Required, MaxLength(20)]
        public string Auditory { get; set; }
        public LessonType LessonType { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int WorkDayId { get; set; }
        public WorkDay WorkDay { get; set; }

        [NotMapped]
        public string TimeInterval {
            get {
                switch (LessonNum) {
                    case 1:
                        return "8:30-10:05";
                    case 2:
                        return "10:15-11:50";
                    case 3:
                        return "12:15-13:50";
                    case 4:
                        return "14:00-15:35";
                    case 5:
                        return "15:45-17:20";
                    case 6:
                        return "17:30-19:05";
                    case 7:
                        return "19:15-20:50";
                    case 8:
                        return "21:00-22:35";
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
                        return "14:00";
                    case 5:
                        return "15:45";
                    case 6:
                        return "17:30";
                    case 7:
                        return "19:15";
                    case 8:
                        return "21:00"; 
                    default:
                        return "поздно";
                }
            }
        }

    }
}
