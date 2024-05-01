using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpkModel;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;
using UpkServices.UI;

namespace UpkViewModel.TeacherReport
{
    public class TeacherReportsViewModel : BaseTeacherViewModel
    {
        public TeacherReportsViewModel(Teacher teacher, IConfigStore configStore) : this(teacher,null, configStore)
        {
        }
        public TeacherReportsViewModel(Teacher teacher, EventHandler<string> defaultEventHandler, IConfigStore configStore) : base(teacher,defaultEventHandler)
        {
            MakeHourReportCommand = new RelayCommand((param) => {
                var fileService = ServiceProvider.GetService<IFileDialogService>() as IFileDialogService;
                if( fileService.ShowSaveFileDialog()) {
                    var s = ServiceProvider.GetService<IMessageService>() as IMessageService;
                    s.ShowDialog("Hello", "Caption", Buttons.YesNoCancel);
                }
            }, false);
        }
        private const int HoursInLesson = 2;
        private int _selectedMonthNum = -1;
        private int _lectionsCount;
        private int _labsCount;
        private int _practicsCount;
        private int _consultationsCount;
        private int _maxHourForReport;

        private IEnumerable<Lesson> _allLessons;
        private Lesson[] _unrecordedLessons;
        #region Properties
        /// <summary>
        /// Номер выбранного месяца
        /// </summary>
        public int SelectedMonthNum
        {
            get => _selectedMonthNum;
            set
            {
                _selectedMonthNum = value;
                OnPropertyChanged();
                LoadStatistic();
            }
        }

        public override Teacher Teacher
        {
            get => base.Teacher;
            set
            {
                if (Teacher != value) {
                    base.Teacher = value;
                    LoadStatistic();
                }
            }
        }

        public int LectionsCount
        {
            get => _lectionsCount;
            set
            {
                _lectionsCount = value;
                OnPropertyChanged();
            }
        }
        public int LabsCount
        {
            get => _labsCount;
            set
            {
                _labsCount = value;
                OnPropertyChanged();
            }
        }
        public int PracticsCount
        {
            get => _practicsCount;
            set
            {
                _practicsCount = value;
                OnPropertyChanged();
            }
        }
        public int ConsultationsCount
        {
            get => _consultationsCount;
            set
            {
                _consultationsCount = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Ограничение на макс. кол-во часов для почасового отчета
        /// </summary>
        public int MaxHourForReport
        {
            get => _maxHourForReport;
            set
            {
                _maxHourForReport = value;
                OnPropertyChanged();
            }
        }

        public Lesson[] UnrecordedLessons
        {
            get => _unrecordedLessons;
            set
            {
                _unrecordedLessons = value;
                OnPropertyChanged();
            }
        }
        private IEnumerable<Lesson> AllLessons
        {
            get => _allLessons;
            set
            {
                _allLessons = value;
                OnPropertyChanged();
            }
        }

        #endregion
        #region Commands

        public RelayCommand MakeHourReportCommand { get; }
        #endregion

        protected override void CheckCommandsAvailability()
        {
            if (MakeHourReportCommand == null) {
                return;
            }
            if (Teacher == null || AllLessons == null || MaxHourForReport <=0 ) {
                MakeHourReportCommand.IsEnabled = false;
            } else {
                MakeHourReportCommand.IsEnabled = true;
            }
            base.CheckCommandsAvailability();
        }

        async void LoadStatistic()
        {
            if (Teacher == null || _selectedMonthNum == -1) {
                return;
            }
            RaiseServiceEvent("Загрузка данных...");
            LectionsCount = 0;
            LabsCount = 0;
            PracticsCount = 0;
            ConsultationsCount = 0;
            UnrecordedLessons = null;
            AllLessons = null;
            IEnumerable<Lesson> queryResult = null;
            await Task.Run(() =>
                {
                    queryResult = LoadLessons();
                    LectionsCount = HoursInLesson * queryResult.Count(l => l.LessonType == LessonType.Lecture);
                    LabsCount = HoursInLesson * queryResult.Count(l => l.LessonType == LessonType.LabWork);
                    PracticsCount = HoursInLesson * queryResult.Count(l => l.LessonType == LessonType.Practical);
                    ConsultationsCount = queryResult.Count(l => l.LessonType == LessonType.Consultation);
                    UnrecordedLessons = queryResult.Where(l => l.LessonType == LessonType.Unknown)
                        .OrderBy(l => l.Date)
                        .ThenBy(l => l.LessonNum)
                        .ToArray();
                });
            AllLessons = queryResult;
            RaiseServiceEvent("");
        }

        /// <summary>
        /// На основании месяца генерирует первую и последнюю дату месяца учебного года.
        /// При этом учитывается текущий месяц учебного года. Выбирать данные можно только для прошедших месяцев.
        /// Если будет в феврале будет сделана попытка выбрать данные за ноябрь, то будут выгружены данные за ноябрь прошлого года
        /// </summary>
        /// <param name="monthNum">номер месяца, отсчитываемый с единицы</param>
        /// <param name="firstDate">первая дата месяца</param>
        /// <param name="lastDate">последняя дата месяца</param>
        void GetDateIntervalFromMonth(int monthNum, out DateTime firstDate, out DateTime lastDate)
        {
            int year = DateTime.Today.Year;
            int currentMonth = DateTime.Today.Month;
            if (monthNum > currentMonth) {
                year--;
            }
            firstDate = new DateTime(year, monthNum, 1);
            lastDate = new DateTime(year, monthNum, DateTime.DaysInMonth(year, monthNum));
        }

        IEnumerable<Lesson> LoadLessons()
        {
            var loaderFactory = new TeacherWorkDaysLoaderFactory(UpkDatabaseContext.Instance);
            GetDateIntervalFromMonth(SelectedMonthNum + 1, out DateTime firstDate, out DateTime lastDate);
            var loader = loaderFactory.GetLoader(Teacher, firstDate, lastDate);
            return loader.Load().SelectMany(wd => wd.Lessons).ToArray();
        }
    }
}
