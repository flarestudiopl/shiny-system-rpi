using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Commons
{
    public class Logger
    {
        public static void Warning(string message, [CallerMemberName]string callerMember = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int callerLineNumber = 0)
        {
            Console.WriteLine($"{GetLineBeginning(callerMember, callerFilePath, callerLineNumber)} WARNING: {message}");
        }

        private static string GetLineBeginning(string callerMember, string callerFilePath, int callerLineNumber)
        {
            return $"[{DateTime.Now.ToLongTimeString()}][{callerMember}@{Path.GetFileName(callerFilePath)}:{callerLineNumber}]";
        }
    }
}
