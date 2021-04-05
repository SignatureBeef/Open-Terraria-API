using System;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffectInstance : IDisposable
    {
        public static SoundEffectInstance[] Array;

        public float Volume { get; set; }
        public float Pan { get; set; }
        public float Pitch { get; set; }
        public SoundState State { get; set; }
        public virtual bool IsLooped { get; set; }

        public virtual void Play() { }

        public void Stop()
        {
            this.Stop(true);
        }

        public void Stop(bool immediate)
        {
        }
        public void Pause() { }
        public void Resume() { }
        public void Dispose() { }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}