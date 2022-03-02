using System;

namespace Microsoft.Xna.Framework.Audio
{
    public class WaveBank : IDisposable
    {
        public static WaveBank[] Array;

        public WaveBank(AudioEngine audioEngine, string nonStreamingWaveBankFilename) { }
        public WaveBank(AudioEngine audioEngine, string streamingWaveBankFilename, int offset, short packetsize) { }

        public bool IsPrepared { get; set; }
        public void Dispose() { }
    }
}