using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsResource
    {
        public void Dispose()
        {
        }

        public GraphicsDevice GraphicsDevice { get; set; }

        public bool IsDisposed { get; set; }

        public string Name { get; set; }

        public object Tag { get; set; }

        public event EventHandler<EventArgs> Disposing
        {
            add { }
            remove { }
        }
    }
}