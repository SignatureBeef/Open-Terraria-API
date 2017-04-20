using System;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.Graphics
{
	[Serializable]
	public sealed class NoSuitableGraphicsDeviceException : Exception
	{
		public NoSuitableGraphicsDeviceException()
		{
		}

		public NoSuitableGraphicsDeviceException(string message) : base(message)
		{
		}

		public NoSuitableGraphicsDeviceException(string message, Exception inner) : base(message, inner)
		{
		}

		private NoSuitableGraphicsDeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
