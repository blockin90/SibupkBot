using System.Diagnostics;
using System.Runtime.CompilerServices;
using UpkServices;

namespace UpkServices
{
    public static class MyTrace
    {
        private static object syncObject = new object();

        public static void WriteLine(string text,
            [CallerMemberName] string methodName = "",
            [CallerFilePathAttribute] string filePath = "",
            [CallerLineNumberAttribute] int lineNumber = 0)
        {
            lock (syncObject) {
                Trace.WriteLine($"{DateInfo.Now};{methodName};{filePath};{lineNumber};{text}");
            }
        }
    }
}
