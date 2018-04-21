using Commons.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Commons
{
    public class Logger
    {
        public static void Error(string message, Exception exception, [CallerMemberName]string callerMember = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int callerLineNumber = 0)
        {
            Console.WriteLine($"{GetLineBeginning(callerMember, callerFilePath, callerLineNumber)} ERROR: {message}, EXCEPTION: {exception}");
        }

        public static void Warning(string message, [CallerMemberName]string callerMember = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int callerLineNumber = 0)
        {
            Console.WriteLine($"{GetLineBeginning(callerMember, callerFilePath, callerLineNumber)} WARNING: {message}");
        }

        public static void Trace(string format, object[] values = null, [CallerMemberName]string callerMember = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int callerLineNumber = 0)
        {
            var message = format.FormatWith(values);

            Console.WriteLine($"{GetLineBeginning(callerMember, callerFilePath, callerLineNumber)} TRACE: {message}");
        }

        private static string GetLineBeginning(string callerMember, string callerFilePath, int callerLineNumber)
        {
            return $"[{DateTime.Now.ToLongTimeString()}][{callerMember}@{Path.GetFileName(callerFilePath)}:{callerLineNumber}]";
        }
    }
}
