using System;

namespace OTAPI.Core.Hooks.Terraria
{
    public static class Main
    {
        public static Func<Boolean> OnStartCommandThread;
        
        internal static bool startDedInput()
        {
            if (OnStartCommandThread != null)
                return OnStartCommandThread.Invoke();
            return true;
        }
    }
}
