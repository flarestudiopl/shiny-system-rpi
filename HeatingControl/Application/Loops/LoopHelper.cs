using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Commons;

namespace HeatingControl.Application.Loops
{
    public static class LoopHelper
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
                                 Logger.Error($"{loopName} loop failed! Pausing for {FailureStopTimeoutMilliseconds / 1000} seconds.", exception);

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
