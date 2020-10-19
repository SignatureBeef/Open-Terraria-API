using System;

namespace SteelSeries.GameSense
{
    public class CriticalMessageIllFormedException : Exception
    {
        public CriticalMessageIllFormedException(string message) : base(message) { }
    }
}
