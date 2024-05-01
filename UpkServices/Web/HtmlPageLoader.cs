using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace UpkServices.Web
{
    /// <summary>
    /// Загрузчик веб страниц по протоколу http
    /// </summary>
    public static class HtmlPageLoader
    {
        /// <summary> кодировка, используемая при чтении страниц сайта</summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.GetEncoding(1251);
        /// <summary>
        /// Величина таймаута выполнения запроса в миллисекундах
        /// </summary>
        public static int Timeout { get; set; } = 10000;

        public static string SendWebRequest(string url, string postData = "")
        {
            //через MakeWebRequest(..) делаем запрос и тут же запрашиваем ответ
            using (WebResponse resp = CewateWebRequest(url, postData).GetResponse())
            using (Stream stream = resp.GetResponseStream()) {
                using (StreamReader sr = new StreamReader(stream)) {
                    return sr.ReadToEnd();
                }
            }
        }
        private static WebRequest CewateWebRequest(string url, string postData )
        {
            WebRequest request = WebRequest.CreateHttp(url);
            request.Timeout = Timeout;
            if (postData != null) {
                WritePostDataToWebRequest(request, postData);
            }
            return request;
        }
        private static void WritePostDataToWebRequest(WebRequest request, string postData)
        {
            byte[] data = DefaultEncoding.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Proxy = null;
            using (Stream stream = request.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
