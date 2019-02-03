using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace UpkServices
{
    public static class TraceTextFormatter
    {
        public static string Format( string text,
            [CallerMemberName] string methodName = "", 
            [CallerFilePathAttribute] string filePath = "", 
            [CallerLineNumberAttribute] int lineNumber = 0)
        {
            return $"{DateInfo.Now};{methodName};{filePath};{lineNumber};{text}";
        }
    }
}
