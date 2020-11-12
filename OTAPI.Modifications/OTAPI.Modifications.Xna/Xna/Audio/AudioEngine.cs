using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.Audio
{
    public class AudioEngine
    {
        public AudioEngine(string settingsFile) { }

        public void Update() { }
    }

    [Serializable]
    public sealed class NoAudioHardwareException : ExternalException
    {
        public NoAudioHardwareException()
        {
        }

        public NoAudioHardwareException(string message)
            : base(message)
        {
        }

        public NoAudioHardwareException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private NoAudioHardwareException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}