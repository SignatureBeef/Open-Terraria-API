using System;
using OTA.Misc;
using System.Collections.Concurrent;

namespace OTA.Protection
{
    public static class IpLimiting
    {
        class IpLimit
        {
            public FixedConcurrentQueue<DateTime> Requests;
            public DateTime? LastLockout;
            public bool JustLockedOut;
        }

        //TODO perhaps a context is required (??), say 1) API, 2) Player Login
        private static ConcurrentDictionary<String, IpLimit> _requestMap = new ConcurrentDictionary<String, IpLimit>();

        //        public static int DelayMinutes { get; set; }
        //
        //        static IpLimiting()
        //        {
        //            DelayMinutes = 10;
        //        }

        public static DateTime? GetLastLockout(string ip)
        {
            IpLimit existing;
            if (_requestMap.TryGetValue(ip, out existing))
            {
                return existing.LastLockout;
            }
            return null;
        }

        public static bool GetJustLockedOut(string ip)
        {
            IpLimit existing;
            if (_requestMap.TryGetValue(ip, out existing))
            {
                return existing.JustLockedOut;
            }
            return false;
        }

        /// <summary>
        /// Register for limiting.
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="maxRequests">Max requests allowed for time frame</param>
        /// <param name="delayMinutes">Time to wait when the limit is reached</param>
        public static bool Register(string ip, int maxRequests, int delayMinutes)
        {
            //The target is to track the latest requests
            //When it's reached don't add any (but ensure it's denied)
            //In addition, when it is reached update all records to the request date. 
            //This way they must certainly wait the duration.

            IpLimit existing;
            if (!_requestMap.TryGetValue(ip, out existing))
            {
                existing = new IpLimit()
                {
                    Requests = new FixedConcurrentQueue<DateTime>(maxRequests)
                };
                if (!_requestMap.TryAdd(ip, existing))
                {
                    //ProgramLog.Error.Log("Failed to increment API request, the request will be denied");
                    return true;
                }
            }

            if (existing.JustLockedOut) existing.JustLockedOut = false;

            if (existing.Requests.IsLimited(delayMinutes))
            {
                //Ensure rejected requests reset the wait time
                existing.Requests.Fill(DateTime.Now);
                existing.LastLockout = DateTime.Now;
                return true;
            }
            existing.Requests.Enqueue(DateTime.Now);
            if (existing.Requests.IsLimited(delayMinutes))
            {
                //Ensure rejected requests reset the wait time
                existing.Requests.Fill(DateTime.Now);
                existing.LastLockout = DateTime.Now;
                existing.JustLockedOut = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the log has too many requests for the time frame.
        /// </summary>
        public static bool IsLimited(this FixedConcurrentQueue<DateTime> queue, int delayMinutes)
        {
            if (queue.Count >= queue.MaxSize)
            {
                DateTime first;
                if (queue.TryPeek(out first))
                {
                    return (DateTime.Now - first).TotalMinutes < delayMinutes;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets all entries to [time]
        /// </summary>
        /// <param name="queue">Queue.</param>
        public static void Fill(this FixedConcurrentQueue<DateTime> queue, DateTime time)
        {
            for (var x = 0; x < queue.MaxSize; x++)
                queue.Enqueue(time);
        }
    }
}