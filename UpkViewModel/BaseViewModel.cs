using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpkViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        readonly SynchronizationContext _synchronizationContext;
        readonly ManualResetEvent _mainThreadSynsEvent;

        public BaseViewModel()
        {
            _synchronizationContext = SynchronizationContext.Current.CreateCopy();
            _mainThreadSynsEvent = new ManualResetEvent(false);
        }

        public void InvokeOnMainThread(Action action)
        {
            try {
                SendOrPostCallback callback = (object state) =>
                {
                    action();
                    _mainThreadSynsEvent.Set();
                };
                _synchronizationContext.Post(callback, null);
                _synchronizationContext.Wait(new[] { _mainThreadSynsEvent.SafeWaitHandle.DangerousGetHandle() }, true, -1);
            } finally {
                _mainThreadSynsEvent.Reset();
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
