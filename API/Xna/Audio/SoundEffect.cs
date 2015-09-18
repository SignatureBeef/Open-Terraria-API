#if XNA_SHIMS
namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffect
    {
        public static SoundEffect[] Array;

        public SoundEffectInstance CreateInstance()
        {
            return default(SoundEffectInstance);
        }
    }
}
#endif