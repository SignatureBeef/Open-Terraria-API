using System;

namespace OTAPI.Core.Hooks.Terraria
{
    public static class Main
    {
        public static bool startDedInputBegin()
        {
            Console.WriteLine("OTAPI - Start ded input");
            return true;
        }

        public static bool startDedInputEnd()
        {
            Console.WriteLine("OTAPI - End ded input");
            return true;
        }
    }
}
