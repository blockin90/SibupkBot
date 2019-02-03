using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UpkModel.Database;
using UpkServices;

namespace UpkViewModel
{
    /// <summary>
    /// Вью модель, загружаемая на главной форме приложения
    /// </summary>
    /// <remarks>
    /// Содержит список преподавателей, ссылки на выбранного преподавателя и на активную дочернюю модель представления
    /// </remarks>
    public class MainTeacherViewModel : INotifyPropertyChanged
    {
        private IEnumerable<Teacher> _teachers;
        private Teacher _selectedTeacher;
        private string _serviceMessage = String.Empty;

        private BaseTeacherViewModel _currentViewModel;
        private List<BaseTeacherViewModel> _childs = new List<BaseTeacherViewModel>();

        public RelayCommand ReloadTeachersCommand { get; private set; }
        public RelayCommand SwitchViewModelCommand { get; private set; }

        private bool _isEnabled = true;

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


        public MainTeacherViewModel()
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
                        CurrentViewModel = Activator.CreateInstance(vmType, SelectedTeacher, new EventHandler<string>(CurrentViewModel_ServiceEvent)) as BaseTeacherViewModel;
                        //CurrentViewModel.ServiceEvent += CurrentViewModel_ServiceEvent;
                        _childs.Add(CurrentViewModel);
                    }
                }
            });
            
            LoadTeachers();
            var lastSelection = Configs.Instance.GetData("SelectedTeacher");
            if (lastSelection != null) {
                SelectedTeacher = _teachers.FirstOrDefault(t => t.Id == int.Parse(lastSelection));
            }
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
                        //сохраняем выбор сотрудника 
                        Configs.Instance["SelectedTeacher"] = SelectedTeacher.Id.ToString();
                        Configs.Instance.SaveChanges();
                    }
                    OnPropertyChanged();
                }
            }
        }

        void LoadTeachers(bool forceFromWeb = false)
        {
            ServiceMessage = "Загрузка списка преподавателей...";
            Task.Run(async () =>
            {
                try {
                    Teachers = await (new TeachersFactory(UpkDatabaseContext.Instance, Configs.Instance)).GetTeachersAsync(forceFromWeb);
                } catch {
                    ServiceMessage = "Невозможно загрузить список преподавателей, ошибки сетевого соединения. Повторите попытку позже.";
                    return;
                }
                ServiceMessage = String.Empty;
            });


        }

        private void CurrentViewModel_ServiceEvent(object sender, string e)
        {
            ServiceMessage = e;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
