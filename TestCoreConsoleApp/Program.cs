using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        const int TimerIntervalMillinseconds = 3000;
        const bool flag = true;

        static async Task Main(string[] args)
        {
            var autoEvent = new AutoResetEvent(false);

            var statusChecker = new StatusChecker();

            Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                              DateTime.Now);
            var stateTimer = new Timer(statusChecker.CheckStatus,
                                       autoEvent, 1000, TimerIntervalMillinseconds);

            // When autoEvent signals, change the period to every half second.
            autoEvent.WaitOne();
            stateTimer.Dispose();
        }

        class StatusChecker
        {
            private int invokeCount = 0;
            private int millisecondsTimeout = 90000;
            private int maxCount;

            public StatusChecker()
            {
                maxCount = millisecondsTimeout / TimerIntervalMillinseconds;
            }


            // This method is called by the timer delegate.
            public void CheckStatus(Object stateInfo)
            {
                AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
                Console.WriteLine("{0} Checking status {1,2}.",
                    DateTime.Now.ToString("h:mm:ss.fff"),
                    (++invokeCount).ToString());


                if (invokeCount == maxCount)
                {
                    // Reset the counter and signal the waiting thread.
                    invokeCount = 0;
                    autoEvent.Set();
                }
            }
        }
    }
}
