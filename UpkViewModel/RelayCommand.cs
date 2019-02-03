using System;
using System.Windows.Input;

namespace UpkViewModel
{
    /// <summary>
    /// Кроссплатформенная реализация команды WPF
    /// </summary>
    /// <remarks>
    /// Реализация взята с https://docs.microsoft.com/en-us/dotnet/standard/cross-platform/using-portable-class-library-with-model-view-view-model
    /// </remarks>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _handler;
        private bool _isEnabled;

        public RelayCommand(Action<object> handler, bool initalState = true)
        {
            _handler = handler;
            IsEnabled = initalState;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled) {
                    _isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }
}
