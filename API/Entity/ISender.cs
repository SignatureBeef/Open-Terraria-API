using System;

namespace OTA
{
    /// <summary>
    /// The bare implementation of a sender
    /// </summary>
    public interface ISender
    {
        string SenderName { get; }

        void SendMessage(string message, int sender = 255, byte R = 255, byte G = 255, byte B = 255);
    }
}