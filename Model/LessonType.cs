using System;
using System.Collections.Generic;
using System.Text;

namespace UpkModel
{
    public enum LessonType
    {
        Unknown,
        Lecture,
        LabWork,
        Practical,
        Exam,
        Consultation,
        NoLesson
    }

    public static class LessonTypeExtension
    {
        public static string GetFriendlyName( this LessonType lessonType)
        {
            switch (lessonType) {
                case LessonType.Consultation:
                    return "конс";
                case LessonType.Exam:
                    return "экз";
                case LessonType.LabWork:
                    return "лаб";
                case LessonType.Lecture:
                    return "лек";
                case LessonType.Practical:
                    return "с";
                case LessonType.NoLesson:
                    return String.Empty;
                default:
                    return "?";
            }
        }
    }
}
