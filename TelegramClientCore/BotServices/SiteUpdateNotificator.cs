using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UpkServices;

namespace TelegramClientCore.BotServices
{
    /// <summary>
    /// Класс, проверяющий сайт на наличие обновлений со страницы 
    /// </summary>
    internal class SiteUpdateNotificator : ISiteUpdateNotificator, IDisposable
    {
        private string _lastSiteUpdateDate;
        private readonly TimeSpan _sleepInterval;
        private readonly string _siteUrl;

        public event EventHandler<EventArgs> OnSiteUpdate;
        CancellationToken _cancellationToken;
        private Task _task;

        private void Run()
        {
            while ( !_cancellationToken.IsCancellationRequested) {
                Thread.Sleep(_sleepInterval);
                DateTime currentTime = DateInfo.Now;
                if( currentTime.Hour >= 23 || currentTime.Hour < 7) {
                    continue;
                }
                string newDate;
                try {
                    //загружаем страничку
                    MyTrace.WriteLine("Загрузка даты обновления сайта");
                    newDate = SiteUpdateDateSelector.GetUpdateDate(_siteUrl);
                } catch (Exception e) {
                    MyTrace.WriteLine(e.GetFullMessage());
                    continue;
                }
                //если дата последнего обновления еще не установлена, устанавливаем
                if (String.IsNullOrEmpty(_lastSiteUpdateDate)) {
                    _lastSiteUpdateDate = newDate;
                } else if (newDate != _lastSiteUpdateDate) {
                    _lastSiteUpdateDate = newDate;
                    //было обновление, уведомляем подписчиков
                    OnSiteUpdate?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public SiteUpdateNotificator(TimeSpan sleepInterval, string siteUrl)
        {
            _task = Task.Run(() => Run());
            _sleepInterval = sleepInterval;
            _siteUrl = siteUrl;
        }




        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    _task?.Dispose();
                }
                disposedValue = true;
            }
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
