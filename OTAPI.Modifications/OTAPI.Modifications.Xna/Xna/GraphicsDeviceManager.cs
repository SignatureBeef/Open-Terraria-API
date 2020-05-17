using Microsoft.Xna.Framework.Graphics;
using System;

namespace Microsoft.Xna.Framework
{
    public class GraphicsDeviceManager
    {
        public GraphicsDeviceManager(Game game) { }

        public bool IsFullScreen { get; set; }
        public int PreferredBackBufferWidth { get; set; }
        public int PreferredBackBufferHeight { get; set; }
        public bool SynchronizeWithVerticalRetrace { get; set; }

        public void ToggleFullScreen() { }
        public void ApplyChanges() { }

        public GraphicsDevice GraphicsDevice { get; set; }
        public GraphicsProfile GraphicsProfile { get; set; }
    }

    public class GraphicsDeviceInformation
    {
        public GraphicsAdapter Adapter { get; set; }

        public GraphicsProfile GraphicsProfile { get; set; }

        public PresentationParameters PresentationParameters { get; set; }

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 0;

        public GraphicsDeviceInformation Clone() => null;
    }

    public class PreparingDeviceSettingsEventArgs : EventArgs
    {
        public GraphicsDeviceInformation GraphicsDeviceInformation { get; }

        public PreparingDeviceSettingsEventArgs(GraphicsDeviceInformation graphicsDeviceInformation) { }
    }
}