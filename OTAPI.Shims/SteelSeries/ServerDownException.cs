using System;

namespace SteelSeries.GameSense
{
    public class ServerDownException : Exception
    {
        public ServerDownException(string message) : base(message) { }
    }
}
