using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsAdapter
    {
        public static bool UseReferenceDevice => false;
        public static bool UseNullDevice => false;

        public IntPtr MonitorHandle => IntPtr.Zero;

        public DisplayModeCollection SupportedDisplayModes => null;

        public DisplayMode CurrentDisplayMode => null;

        public bool IsWideScreen => false;

        public bool IsDefaultAdapter => false;

        public int Revision => 0;

        public int SubSystemId => 0;

        public int DeviceId => 0;

        public int VendorId => 0;

        public string DeviceName => null;

        public string Description => null;

        public static GraphicsAdapter DefaultAdapter => null;

        public static ReadOnlyCollection<GraphicsAdapter> Adapters => null;


        public bool QueryBackBufferFormat(GraphicsProfile graphicsProfile, SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount, out SurfaceFormat selectedFormat, out DepthFormat selectedDepthFormat, out int selectedMultiSampleCount)
        {
            selectedFormat = default(SurfaceFormat);
            selectedDepthFormat = default(DepthFormat);
            selectedMultiSampleCount = 0;
            return false;
        }

        public bool QueryRenderTargetFormat(GraphicsProfile graphicsProfile, SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount, out SurfaceFormat selectedFormat, out DepthFormat selectedDepthFormat, out int selectedMultiSampleCount)
        {
            selectedFormat = default(SurfaceFormat);
            selectedDepthFormat = default(DepthFormat);
            selectedMultiSampleCount = 0;
            return false;
        }

        public bool IsProfileSupported(GraphicsProfile graphicsProfile) => false;
    }
}