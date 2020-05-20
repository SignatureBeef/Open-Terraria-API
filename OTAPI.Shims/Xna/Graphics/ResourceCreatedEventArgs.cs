using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public sealed class ResourceCreatedEventArgs : EventArgs
	{
		internal object _resource;

		public object Resource
		{
			get
			{
				return this._resource;
			}
		}

		internal ResourceCreatedEventArgs(object resource)
		{
			this._resource = resource;
		}
	}
}