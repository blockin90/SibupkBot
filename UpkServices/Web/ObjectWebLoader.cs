using HtmlAgilityPack;
using UpkModel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UpkModel;

namespace UpkServices.Web
{
    public static class ObjectWebLoader
    {
        /// <summary>
        /// Загрузка объектов типа T со страницы, которая является страницей по умолчанию для данного типа объектов 
        /// </summary>
        /// <returns>множество объектов типа T</returns>
        public static IEnumerable<T> Load<T>(string postParameters="") where T : class
        {
            Type type = typeof(T);
            object result = null;
            if (type == typeof(StudentWorkDay)) {
                if (String.IsNullOrEmpty(postParameters)) {
                    throw new ArgumentException("by loading work days post parameters must not be empty ");
                }
                result = Load<StudentWorkDay>(
                    HtmlNodeParsers.DecodeStudentWorkDay,
                    HtmlNodeParsers.StudentBaseUrl,
                    HtmlNodeParsers.StudentWorkDayXPath,
                    postParameters);
            } else if (type == typeof(WorkDay)) {
                if (String.IsNullOrEmpty(postParameters)) {
                    throw new ArgumentException("by loading work days post parameters must not be empty ");
                }
                result = Load<WorkDay>(
                    HtmlNodeParsers.DecodeWorkDay,
                    HtmlNodeParsers.TeacherBaseUrl,
                    HtmlNodeParsers.WorkDayXPath,
                    postParameters);
            } else if (type == typeof(WeekInterval)) {
                if (String.IsNullOrEmpty(postParameters)) {
                    throw new ArgumentException("by loading week intervals post parameters must not be empty ");
                }
                result = Load<WeekInterval>(
                    HtmlNodeParsers.DecodeTimeInterval,
                    HtmlNodeParsers.TeacherBaseUrl,
                    HtmlNodeParsers.DateIntervalsXpath,
                    postParameters);
            } else if( type == typeof(UpkModel.Database.Group)) {
                if (String.IsNullOrEmpty(postParameters)) {
                    throw new ArgumentException("by loading groups post parameters must not be empty ");
                }
                result = Load<UpkModel.Database.Group>(
                    HtmlNodeParsers.DecodeGroup,
                    HtmlNodeParsers.StudentBaseUrl,
                    HtmlNodeParsers.GroupsXpath,
                    postParameters);
            } else if (type == typeof(Teacher)) {
                result = Load<Teacher>(
                    HtmlNodeParsers.DecodeTeacher, 
                    HtmlNodeParsers.TeacherBaseUrl, 
                    HtmlNodeParsers.TeacherXpath);
            } else if (type == typeof(Department)) {
                result = Load<Department>(
                    HtmlNodeParsers.DecodeDepartment,
                    HtmlNodeParsers.TeacherBaseUrl,
                    HtmlNodeParsers.DepartmentXPath);
            } 
            return (result as IEnumerable<T>) ?? throw new NotImplementedException();
        }
        /// <summary>
        /// Выполняет загрузку страницы по указанному адресу и возврат объектов, соответствующих заданным узлам html-страницы
        /// </summary>
        /// <param name="decoder">функция-преобразователь html-блока в объект типа T</param>
        /// <param name="url">адрес страницы для загрузки</param>
        /// <param name="xPath">xpath к узлам сайта, которые необходимо получить</param>
        /// <param name="postData">POST параметры для передачи в запрос, если необходимо</param>
        /// <returns>множество объектов типа T</returns>
        private static IEnumerable<T> Load<T>(Func<HtmlNode, T> decoder, string url, string xPath, string postData = "") where T : class
        {
            HtmlDocument doc = new HtmlDocument();
            string html = HtmlPageLoader.SendWebRequest(url,postData);
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(xPath);
            if( nodes == null) {
                return new T[0];
            }
            return nodes.Select(decoder).Where( e => e != null );
        }
        /// <summary>
        /// Парсинг переданной страницы и возврат объектов, соответствующих заданным узлам html-страницы
        /// </summary>
        /// <param name="decoder">функция-преобразователь html-блока в объект типа T</param>
        /// <param name="xPath">xpath к узлам сайта, которые необходимо получить</param>
        /// <param name="htmlContent">содержимое html-странички для парсинга</param>
        /// <returns>множество объектов типа T</returns>
        private static IEnumerable<T> Load<T>(Func<HtmlNode, T> decoder, string xPath, HtmlDocument htmlContent) where T : class
        {
            var nodes = htmlContent.DocumentNode.SelectNodes(xPath);
            return nodes.Select(decoder).Where(e => e != null);
        }
    }
}
