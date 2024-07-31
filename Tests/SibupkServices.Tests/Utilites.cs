using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SibupkServices.Tests
{
    internal class Utilites
    {
        public static string GetHtmlPage(string resourceName)
        {
            //SibupkServices.Tests.TestData.Input.ScheduleUpdatePage.html
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SibupkServices.Tests.TestData.Input.{resourceName}.html")) {
                if(stream == null) {
                    throw new Exception($"Resource {resourceName} not found!");
                }
                using (var reader = new StreamReader(stream!)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
