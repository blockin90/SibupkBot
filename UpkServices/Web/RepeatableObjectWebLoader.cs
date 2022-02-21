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
                    LogException(nex, typeof(T), postData);
                    throw;
                } catch (System.Exception e) {
                    LogException(e, typeof(T), postData);                    
                    Thread.Sleep(_sleepInterval);
                    continue;
                }
            }
            throw new TimeoutException("Время ожидания загрузки истекло, повторите попытку позже");
        }
        private static void LogException( Exception exception, Type type, string postData)
        {
            MyTrace.WriteLine($"{exception.GetFullMessage()}{Environment.NewLine}Request was made for {type.Name} with params: {postData}");
        }
    }
}
