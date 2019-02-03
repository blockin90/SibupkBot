using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UpkServices;
using UpkServices.Web;

namespace TelegramClientCore.BotServices
{
    /// <summary>
    /// Класс, выбирающий дату последнего обновления с переданной страницы
    /// </summary>
    class SiteUpdateDateSelector
    {
        /// <summary>
        /// Получение даты последнего обновления сайта
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateDate(string url)
        {
            for (int i = 0; i < 10; ++i) {
                try {
                    string result = String.Empty;
                    using (WebResponse resp = WebRequest.CreateHttp(url).GetResponse())
                    using (Stream stream = resp.GetResponseStream()) {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8)) {
                            result = sr.ReadToEnd();
                        }
                    }
                    string searchString = "Последнее обновление:";
                    int index = result.IndexOf(searchString);
                    result = result.Substring(index + searchString.Length, 15);
                    Regex r = new Regex(@"\d+\.\d+\.\d+");
                    var match = r.Match(result);
                    if(match.Success) {
                        return match.Value;
                    }
                } catch (Exception e){
                    MyTrace.WriteLine(e.Message);
                    Thread.Sleep(1000);
                    continue;
                }
            }
            throw new TimeoutException();
        }
    }
}
