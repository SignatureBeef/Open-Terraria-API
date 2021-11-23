namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		internal static void SendUnknownPacket
		(
			int bufferId,
			System.IO.BinaryWriter writer,
			int msgType,
			int remoteClient,
			int ignoreClient,
            global::Terraria.Localization.NetworkText text,
			int number,
			float number2,
			float number3,
			float number4,
			int number5,
			int number6,
			int number7
		)
		{
			Hooks.Net.SendUnknownPacket?.Invoke
			(
				bufferId,
				writer,
				msgType,
				remoteClient,
				ignoreClient,
				text,
				number,
				number2,
				number3,
				number4,
				number5,
				number6,
				number7
			);
		}
	}
}
