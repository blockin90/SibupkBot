using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace UpkViewModel
{
    /// <summary>
    /// Вью модель, загружаемая на главной форме приложения
    /// </summary>
    /// <remarks>
    /// Содержит список преподавателей, ссылки на выбранного преподавателя и на активную дочернюю модель представления
    /// </remarks>
    public class MainTeacherViewModel : BaseViewModel
    {
        private IEnumerable<Teacher> _teachers;
        private Teacher _selectedTeacher;
        private string _serviceMessage = String.Empty;

        private BaseTeacherViewModel _currentViewModel;
        private List<BaseTeacherViewModel> _childs = new List<BaseTeacherViewModel>();

        public RelayCommand ReloadTeachersCommand { get; private set; }
        public RelayCommand SwitchViewModelCommand { get; private set; }

        private bool _isEnabled = true;
        private readonly IConfigStore _configStore;

        /// <summary>
        /// Состояние вью модели - true, если данные загружены и ожидается ввод пользователя, false, если идет загрузка данных
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }


        public MainTeacherViewModel(IConfigStore configStore)
        {
            ReloadTeachersCommand = new RelayCommand((param) => LoadTeachers(true));
            SwitchViewModelCommand = new RelayCommand((param) =>
            {
                Type vmType = param as Type;
                if (vmType != null) {
                    var OldWm = _childs.FirstOrDefault(vm => vm.GetType() == vmType);
                    if (OldWm != null) {
                        CurrentViewModel = OldWm;
                        CurrentViewModel.Teacher = SelectedTeacher;
                    } else {
                        CurrentViewModel = Activator.CreateInstance(vmType, SelectedTeacher, new EventHandler<string>(CurrentViewModel_ServiceEvent), _configStore) as BaseTeacherViewModel;
                        //CurrentViewModel.ServiceEvent += CurrentViewModel_ServiceEvent;
                        _childs.Add(CurrentViewModel);
                    }
                }
            });
            this._configStore = configStore;
            LoadTeachers(false);
        }

        public BaseTeacherViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public String ServiceMessage
        {
            get { return _serviceMessage; }
            set
            {
                _serviceMessage = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Teacher> Teachers
        {
            get
            {
                return _teachers;
            }
            set
            {
                _teachers = value;
                OnPropertyChanged();

            }
        }

        public Teacher SelectedTeacher
        {
            get
            {
                return _selectedTeacher;
            }
            set
            {
                if (value != _selectedTeacher) {
                    _selectedTeacher = value;
                    if (CurrentViewModel != null) {
                        CurrentViewModel.Teacher = value;
                    }
                    //сохраняем выбор сотрудника 
                    _configStore["SelectedTeacher"] = SelectedTeacher.Id.ToString();
                    _configStore.SaveChanges();
                    OnPropertyChanged();
                }
            }
        }

        public void LoadTeachers(bool forceFromWeb = false)
        {
            ServiceMessage = "Загрузка списка преподавателей...";
            Task.Run(async () =>
            {
                var teachers = await (new TeachersFactory(UpkDatabaseContext.Instance)).GetTeachersAsync(forceFromWeb);
                InvokeOnMainThread(() => Teachers = teachers);
            }).ContinueWith(taskResult =>
            {
                if (taskResult.Exception != null) {
                    InvokeOnMainThread(() => ServiceMessage = $"Не удалось загрузить список преподавателей: {taskResult.Exception}");
                } else {
                    InvokeOnMainThread(() => ServiceMessage = string.Empty);
                    if (SelectedTeacher == null) {
                        var lastSelection = _configStore.GetDataAsString("SelectedTeacher");
                        if (lastSelection != null) {
                            InvokeOnMainThread(() => SelectedTeacher = _teachers.FirstOrDefault(t => t.Id == int.Parse(lastSelection)));
                        }
                    }
                }
            }).ContinueWith( taskResult =>
            {
                var scheduleViewModel =  new TeacherScheduleViewModel(SelectedTeacher, new EventHandler<string>(CurrentViewModel_ServiceEvent), _configStore);
                InvokeOnMainThread(() => CurrentViewModel = scheduleViewModel);
                _childs.Add(CurrentViewModel);
            });
        }

        private void CurrentViewModel_ServiceEvent(object sender, string e)
        {
            ServiceMessage = e;
        }
    }
}
