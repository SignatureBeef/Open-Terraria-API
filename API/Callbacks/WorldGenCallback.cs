using System;
using Terraria;

namespace OTA.Callbacks
{
    public static class WorldGenCallback
    {
        public static bool OnHardModeTileUpdate(int x, int y, int type)
        {
//            Logging.ProgramLog.Admin.Log("Hard mode tile called");
            return false;
        }
    }
}

