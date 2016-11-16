using System;

namespace Microsoft.Xna.Framework
{
	public class GameTime
	{
		public GameTime() { }
		public TimeSpan ElapsedGameTime { get; set; }
		public TimeSpan TotalGameTime { get; set; }
		public bool IsRunningSlowly { get; set; }

		public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
		{
			this.TotalGameTime = totalGameTime;
			this.ElapsedGameTime = elapsedGameTime;
			this.IsRunningSlowly = isRunningSlowly;
		}

		public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime) : this(totalGameTime, elapsedGameTime, false)
		{
		}
	}
}