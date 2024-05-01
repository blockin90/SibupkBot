using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using UpkModel.Database;
using UpkModel.Database.Schedule;
using UpkServices;

namespace UpkViewModel
{
    public class BaseTeacherViewModel : INotifyPropertyChanged
    {
        public BaseTeacherViewModel(Teacher teacher)
        {
            Teacher = teacher;
        }
        public BaseTeacherViewModel(Teacher teacher, EventHandler<string> defaultEventHandler)
        {
            if (defaultEventHandler != null) {
                ServiceEvent += defaultEventHandler;
            }
            Teacher = teacher;
        }
        
        private Teacher _teacher;
        public virtual Teacher Teacher { get => _teacher; set => _teacher = value; }

        /// <summary>
        /// Событие для передачи сервисных сообщений в базовую модель
        /// </summary>
        public event EventHandler<string> ServiceEvent;

        /// <summary>
        /// Возбудить сервисное сообщения для обработки родительским представлением
        /// </summary>
        /// <param name="message"></param>
        protected void RaiseServiceEvent(string message)
        {
            ServiceEvent?.Invoke(this, message);
        }

        /// <summary>
        /// Проверка доступности команд. Добавьте в этот метод логику проверки выполнимости команд. 
        /// </summary>
        protected virtual void CheckCommandsAvailability()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            
            CheckCommandsAvailability();
        }
    }
}
