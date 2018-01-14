using System;
using System.Runtime.CompilerServices;

namespace Commons
{
    public class Logger
    {
        public static void Warning(string message, [CallerMemberName]string callerMember = null)
        {
            Console.WriteLine($"[{callerMember}] WARNING: {message}");
        }
    }
}
