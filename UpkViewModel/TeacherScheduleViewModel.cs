using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using UpkServices;
using UpkModel.Database;
using UpkModel;

namespace UpkViewModel
{
    public class TeacherScheduleViewModel : BaseTeacherViewModel
    {
        IEnumerable<DateTime> _selectedDates;
        /// <summary>
        /// Расписание, загруженное с сайта
        /// </summary>
        IEnumerable<WorkDay> _schedule;
        /// <summary>
        /// Расписание для отображения с учетом параметра "Показывать окна в расписании"
        /// </summary>
        IEnumerable<WorkDay> _scheduleView;
        /// <summary>
        /// Показывать окна в расписании?
        /// </summary>
        bool _isEmptyLessonsVisible;

        #region Properties
        /// <summary>
        /// Расписание, загруженное с сайта
        /// </summary>
        private IEnumerable<WorkDay> Schedule
        {
            get
            {
                return _schedule;
            }
            set
            {
                _schedule = value;
                if (IsEmptyLessonsVisible) {
                    ScheduleView = AddEmptyLessonsToSchedule();
                } else {
                    ScheduleView = value;
                }
            }
        }
        /// <summary>
        /// Показывать окна в расписании?
        /// </summary>
        public bool IsEmptyLessonsVisible
        {
            get { return _isEmptyLessonsVisible; }
            set
            {
                if (value) {
                    ScheduleView = AddEmptyLessonsToSchedule();
                } else {
                    ScheduleView = Schedule;
                }
                _isEmptyLessonsVisible = value;
                Configs.Instance["IsEmptyLessonsVisible"] = value.ToString();
                Configs.Instance.SaveChanges();
            }
        }
        /// <summary>
        /// Расписание для отображения с учетом параметра "Показывать окна в расписании"
        /// </summary>
        public IEnumerable<WorkDay> ScheduleView
        {
            get
            {
                return _scheduleView;
            }
            set
            {
                _scheduleView = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Выбранные даты
        /// </summary>
        public IEnumerable<DateTime> SelectedDates
        {
            get { return _selectedDates; }
            set
            {
                if (value.First() > value.Last()) {
                    _selectedDates = value.Reverse();
                } else {
                    _selectedDates = value;
                }
                if (Teacher != null) {
                    LoadSchedule();
                    //Task.Run(() => LoadSchedule());
                }
                Configs.Instance["TeacherFirstDate"] = _selectedDates.First().ToShortDateString();
                Configs.Instance["TeacherLastDate"] = _selectedDates.Last().ToShortDateString();
                Configs.Instance.SaveChanges();
                OnPropertyChanged();
            }
        }

        public override Teacher Teacher
        {
            get => base.Teacher;
            set
            {
                if (Teacher != value) {
                    base.Teacher = value;
                    LoadSchedule();
                }
                //Task.Run(() => LoadSchedule());
            }
        }
        #endregion

        public TeacherScheduleViewModel(Teacher teacher)
            : this(teacher,null)
        {
        }
        public TeacherScheduleViewModel(Teacher teacher,EventHandler<string> defaultEventHandler)
            : base(teacher, defaultEventHandler)
        {
            try {
                SelectedDates = new[] {
                    DateTime.Parse( Configs.Instance["TeacherFirstDate"]),
                    DateTime.Parse( Configs.Instance["TeacherLastDate"])
                };
            } catch {
                SelectedDates = new[] { DateTime.Today, DateTime.Today.AddDays(7) };
            }
            var emptyLessVisibility = Configs.Instance["IsEmptyLessonsVisible"];
            if ( !String.IsNullOrEmpty( emptyLessVisibility)) {
                IsEmptyLessonsVisible = bool.Parse(emptyLessVisibility);
            }
        }

        async void LoadSchedule()
        {
            if (Teacher != null && SelectedDates != null) {
                RaiseServiceEvent("Загрузка расписания...");
                Schedule = null;
                try {
                    var loaderFactory = new TeacherWorkDaysLoaderFactory(UpkDatabaseContext.Instance, Configs.Instance);
                    var loader = loaderFactory.GetLoader(Teacher, SelectedDates.First(), SelectedDates.Last());
                    Schedule = await loader.LoadAsync();
                } catch (TimeoutException) {   //ошибки соединения
                    RaiseServiceEvent("Невозможно загрузить расписание. Возможны неполадки сетевого соединения.");
                    return;
                }
                if (Schedule.Count() == 0) {
                    RaiseServiceEvent("На выбранный период расписание отсутствует");
                } else {
                    RaiseServiceEvent(String.Empty);
                }
            }
        }

        int GetLessonsCount(WorkDay workDay)
        {
            return workDay.Lessons.Last().LessonNum - (workDay.Lessons.First().LessonNum - 1);
        }

        WorkDay AddEmptyLessonsToWorkDay(WorkDay workDay)
        {
            WorkDay wd = workDay.Copy();
            var result = new List<Lesson>(GetLessonsCount(wd));
            for (int i = 0; i < wd.Lessons.Count - 1; i++) {
                /*последовательно перебираем пары, как только разница между номером текущей и 
                след. пары больше 1, добавляем пустыее пары*/
                result.Add(wd.Lessons[i]);
                while ((result.Last().LessonNum + 1) != wd.Lessons[i + 1].LessonNum) {
                    result.Add(new Lesson() { LessonNum = result.Last().LessonNum + 1, LessonType = LessonType.NoLesson });
                }
            }
            result.Add(wd.Lessons.Last());
            wd.Lessons = result;
            return wd;
        }

        IEnumerable<WorkDay> AddEmptyLessonsToSchedule()
        {
            //если кол-во пар равно разнице между номером последней и первой, то окон нет
            //берем оригинальный workDay, иначе добавляем пустые пары
            var result = Schedule?.Select(
                wd => (wd.Lessons.Count == GetLessonsCount(wd)) ? wd : AddEmptyLessonsToWorkDay(wd))
                .ToArray();
            return result;
        }
    }
}
