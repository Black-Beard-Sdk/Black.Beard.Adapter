using Bb.Servers.Web;

namespace Bb.Adapter
{
    public static class ServiceRunnerBaseExtensions
    {

        /// <summary>
        /// Runs asynchronous service
        /// </summary>
        /// <param name="waitRunning">if set to <c>true</c> [wait service running].</param>
        /// <returns></returns
        public static T Start<T>(this T self, bool waitRunning = true)
            where T : ServiceRunnerBase
        {

            self.RunAsync();

            if (waitRunning)
                while (self.Status != ServiceRunnerStatus.Running)
                {
                    Thread.Sleep(0);
                }

            return self;

        }

        /// <summary>
        /// wait the predicate is true before continue
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Wait<T>(this T self, Func<ServiceRunnerBase, bool> predicate)
            where T : ServiceRunnerBase
        {

            while (predicate(self))
            {
                Thread.Sleep(0);
            }

            return self;

        }

        /// <summary>
        /// Runs asynchronous service
        /// </summary>
        /// <param name="waitRunning">if set to <c>true</c> [wait service running].</param>
        /// <returns></returns
        public static T RunAsync<T>(this T self)
            where T : ServiceRunnerBase
        {

            if (self.Status != ServiceRunnerStatus.Stopped)
                throw new InvalidOperationException("Service is already running");

            Task.Run(() => self.Run(), self.CancellationToken);

            return self;

        }

    }

}