using Commons.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Commons
{
    public class Logger
    {
        private const int MaxLastMessages = 10;

        public static Queue<LoggerMessage> LastMessages { get; } = new Queue<LoggerMessage>();

        public static void Error(string message, Exception exception, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            InternalWrite(GetLineBeginning(callerMember, callerFilePath, callerLineNumber), $"{message}, EXCEPTION: {exception}", Severity.Error);
        }

        public static void Warning(string format, object[] values = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var message = format.FormatWith(values);

            InternalWrite(GetLineBeginning(callerMember, callerFilePath, callerLineNumber), message, Severity.Warning);
        }

        public static void Info(string format, object[] values = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var message = format.FormatWith(values);

            InternalWrite(GetLineBeginning(callerMember, callerFilePath, callerLineNumber), message, Severity.Info);
        }

        public static void Trace(string format, object[] values = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var message = format.FormatWith(values);

            InternalWrite(GetLineBeginning(callerMember, callerFilePath, callerLineNumber), message, Severity.Trace);
        }

        private static void InternalWrite(string source, string content, Severity severity)
        {
            var now = DateTime.Now;

#if DEBUG
            Console.WriteLine($"[{now.ToLongTimeString()}] {source} {severity}: {content}");
#endif

            EmitNonTrace(content, severity, now);
        }

        private static void EmitNonTrace(string content, Severity severity, DateTime now)
        {
            if (severity != Severity.Trace)
            {
                LastMessages.Enqueue(new LoggerMessage
                                     {
                                         Timestamp = now,
                                         Content = content,
                                         Severity = severity
                                     });

                if (LastMessages.Count > MaxLastMessages)
                {
                    LastMessages.Dequeue();
                }
            }
        }

        private static string GetLineBeginning(string callerMember, string callerFilePath, int callerLineNumber)
        {
            return $"[{callerMember}@{Path.GetFileName(callerFilePath)}:{callerLineNumber}]";
        }

        public class LoggerMessage
        {
            public DateTime Timestamp { get; set; }
            public string Content { get; set; }
            public Severity Severity { get; set; }
        }

        public enum Severity
        {
            Error = 1,
            Warning = 2,
            Info = 3,
            Trace = 4
        }
    }
}
