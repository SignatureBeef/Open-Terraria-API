/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
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