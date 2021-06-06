using System;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed class DynamicSoundEffectInstance : SoundEffectInstance
    {
        public int PendingBufferCount { get; set; }

        public override bool IsLooped { get; set; }

        public event EventHandler<EventArgs> BufferNeeded;

        public DynamicSoundEffectInstance(
            int sampleRate,
            AudioChannels channels
        ) : base() { }


        public TimeSpan GetSampleDuration(int sizeInBytes) => TimeSpan.MinValue;

        public int GetSampleSizeInBytes(TimeSpan duration) => 0;

        public override void Play() { }

        public void SubmitBuffer(byte[] buffer) { }

        public void SubmitBuffer(byte[] buffer, int offset, int count) { }

        public void SubmitFloatBufferEXT(float[] buffer) { }

        public void SubmitFloatBufferEXT(float[] buffer, int offset, int count) { }

        protected override void Dispose(bool disposing) { }
    }
}