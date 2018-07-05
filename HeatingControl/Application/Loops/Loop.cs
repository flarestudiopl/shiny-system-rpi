using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Commons.Extensions;
using Commons.Localization;

namespace HeatingControl.Application.Loops
{
    public static class Loop
    {
        private const int FailureStopTimeoutMilliseconds = 6000;

        public static void Start(string loopName, int intervalMilliseconds, Action action, CancellationToken cancellationToken)
        {
            Task.Run(
                     async () =>
                     {
                         while (true)
                         {
                             cancellationToken.ThrowIfCancellationRequested();

                             var stopwatch = Stopwatch.StartNew();

                             try
                             {
                                 action();
                             }
                             catch (Exception exception)
                             {
                                 Logger.Error(Localization.NotificationMessage.LoopFailed.FormatWith(loopName, FailureStopTimeoutMilliseconds / 1000), exception);

                                 await Task.Delay(FailureStopTimeoutMilliseconds, cancellationToken);
                             }

                             Logger.Trace("{0} loop took {1} ms.", new object[] { loopName, stopwatch.ElapsedMilliseconds });

                             await Task.Delay(intervalMilliseconds, cancellationToken);
                         }
                     },
                     cancellationToken);
        }
    }
}
