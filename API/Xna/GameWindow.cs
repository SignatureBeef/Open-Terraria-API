#if XNA_SHIMS
using System;
using OTA;

namespace Microsoft.Xna.Framework
{
    public class GameWindow
    {
        public string Title
        {
            get
            { return Console.Title; }
            set
            {
                SetTitle(value);
            }
        }

        public static void SetTitle(string title)
        {
        }

        public IntPtr Handle { get; set; }

        public bool AllowUserResizing { get; set; }
    }
}
#endif