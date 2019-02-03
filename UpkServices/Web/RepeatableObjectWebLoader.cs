using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace UpkServices.Web
{
    /// <summary>
    /// Загрузчик данных с сайта СибУПК с возможностью повторных загрузок при неудаче
    /// </summary>
    class RepeatableObjectWebLoader
    {
        private readonly int _maxAttempts;
        private readonly int _sleepInterval;
        public RepeatableObjectWebLoader(int maxAttempts, int sleepInterval)
        {
            _maxAttempts = maxAttempts;
            _sleepInterval = sleepInterval;
        }
        public IEnumerable<T> Load<T>(string postData = "") where T : class
        {
            int attempts = _maxAttempts;
            while (attempts-- > 0) {
                try {
                    return ObjectWebLoader.Load<T>(postData);
                } catch (NotImplementedException nex) {
                    MyTrace.WriteLine(nex.Message);
                    throw;
                } catch (System.Exception e) {
                    MyTrace.WriteLine(e.Message);
                    Thread.Sleep(_sleepInterval);
                    continue;
                }
            }
            throw new TimeoutException("Время ожидания загрузки истекло, повторите попытку позже");
        }
    }
}
