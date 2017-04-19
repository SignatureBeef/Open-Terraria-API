namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsAdapter
    {
        public static GraphicsAdapter DefaultAdapter
        {
            get { return default(GraphicsAdapter); }
        }

        public DisplayMode CurrentDisplayMode
        {
            get { return default(DisplayMode); }
        }

        public DisplayModeCollection SupportedDisplayModes
        {
            get { return default(DisplayModeCollection); }
        }

        //[return: MarshalAs(UnmanagedType.U1)]
        public bool IsProfileSupported(GraphicsProfile graphicsProfile)
        {
            return false;
        }
    }
}