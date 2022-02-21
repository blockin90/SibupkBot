using System;
using System.Collections.Generic;
using System.Text;

namespace UpkServices
{

    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception exception)
        {
            return $"{exception.Message}{Environment.NewLine}{exception.StackTrace}" +
                $"{(exception.InnerException != null ? (Environment.NewLine+ Environment.NewLine + exception.InnerException.GetFullMessage()) : string.Empty)}";
        }
    }
}
