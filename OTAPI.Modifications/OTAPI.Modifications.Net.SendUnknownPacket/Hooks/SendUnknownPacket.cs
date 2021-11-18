namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Net
		{
			#region Handlers
			public delegate void SendUnknownPacketHandler
			(
				int bufferId,
				System.IO.BinaryWriter writer,
				int msgType,
				int remoteClient,
				int ignoreClient,
                Terraria.Localization.NetworkText text,
				int number,
				float number2,
				float number3,
				float number4,
				int number5,
				int number6,
				int number7,
				float number8
			);
			#endregion

			/// <summary>
			/// Occurs when a unknown packet is required to be written to the BinaryWriter.
			/// </summary>
			public static SendUnknownPacketHandler SendUnknownPacket;
		}
	}
}
