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