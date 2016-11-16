using System;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        public string RootDirectory { get; set; }

		public IServiceProvider ServiceProvider { get; }

		public ContentManager(IServiceProvider serviceProvider) : this(serviceProvider, string.Empty)
		{
		}

		public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
		{ }

		public T Load<T>(string path)
        {
            return default(T);
        }
        //public Effect Load<Effect>(string path)
        //{
        //    return default(Effect);
        //}
    }
}