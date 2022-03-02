using System;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundBank : IDisposable
    {
        public static SoundBank[] Array;

        public SoundBank(AudioEngine audioEngine, string filename) { }

        public Cue GetCue(string name) { return default(Cue); }
        public void Dispose() { }
    }
}