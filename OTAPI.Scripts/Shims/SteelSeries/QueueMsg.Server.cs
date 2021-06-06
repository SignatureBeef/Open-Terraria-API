using System;

namespace SteelSeries.GameSense
{
    public abstract class QueueMsg
    {
        public abstract object data { get; set; }

        public abstract Uri uri { get; }

        public abstract bool IsCritical();

        protected object _data;
    }
}
