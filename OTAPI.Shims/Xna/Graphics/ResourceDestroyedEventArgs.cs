using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public sealed class ResourceDestroyedEventArgs : EventArgs
	{
		internal object _tag;

		internal string _name;

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public object Tag
		{
			get
			{
				return this._tag;
			}
		}

		internal ResourceDestroyedEventArgs(string name, object tag)
		{
			this._tag = tag;
			this._name = name;
		}
	}
}