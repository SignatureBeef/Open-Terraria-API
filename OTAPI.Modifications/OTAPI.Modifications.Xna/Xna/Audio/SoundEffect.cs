using System.IO;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffect
    {
        public static SoundEffect[] Array;

        public SoundEffectInstance CreateInstance()
        {
            return default(SoundEffectInstance);
        }

        public static SoundEffect FromStream(Stream stream) => null;
    }
}