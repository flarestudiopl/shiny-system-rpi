using Commons.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Commons
{
    public class Logger
    {
        private const int MaxLastMessages = 10;
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();

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

        public static void DebugWithData(string message, object data, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var messageWithData = $"{message} {JsonConvert.SerializeObject(data)}";

            InternalWrite(GetLineBeginning(callerMember, callerFilePath, callerLineNumber), messageWithData, Severity.Debug);
        }

        private static void InternalWrite(string source, string content, Severity severity)
        {
            var now = DateTime.UtcNow;

#if DEBUG
            Console.WriteLine($"[{now.ToLongTimeString()}] {source} {severity}: {content}");
#endif
            NlogWrite(source, content, severity);
            EmitNonTrace(content, severity, now);
        }

        private static void NlogWrite(string source, string content, Severity severity)
        {
            var message = $"{source}: {content}";

            switch (severity)
            {
                case Severity.Error:
                    NLogger.Error(message);
                    break;
                case Severity.Warning:
                    NLogger.Warn(message);
                    break;
                case Severity.Info:
                    NLogger.Info(message);
                    break;
                case Severity.Debug:
                    NLogger.Debug(message);
                    break;
                case Severity.Trace:
                    NLogger.Trace(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private static void EmitNonTrace(string content, Severity severity, DateTime now)
        {
            if (severity != Severity.Trace && 
                severity != Severity.Debug)
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
            Debug = 4,
            Trace = 5
        }
    }
}
