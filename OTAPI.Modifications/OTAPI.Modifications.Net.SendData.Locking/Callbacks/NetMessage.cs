using System.IO;
using System.IO.Compression;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		internal static int CompressTileBlock(int xStart, int yStart, short width, short height, BinaryWriter writer, int bufferStart)
		{
			using (MemoryStream rawStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(rawStream))
				{
					binaryWriter.Write(xStart);
					binaryWriter.Write(yStart);
					binaryWriter.Write(width);
					binaryWriter.Write(height);
					global::Terraria.NetMessage.CompressTileBlock_Inner(binaryWriter, xStart, yStart, (int)width, (int)height);
					rawStream.Position = 0L;

					MemoryStream compressedStream = new MemoryStream();
					using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
					{
						rawStream.CopyTo(deflateStream);
						deflateStream.Flush();
						deflateStream.Close();
						deflateStream.Dispose();
					}

					if (rawStream.Length <= compressedStream.Length)
					{
						rawStream.Position = 0L;
						writer.Write((byte)0);
						writer.Write(rawStream.GetBuffer());
					}
					else
					{
						compressedStream.Position = 0L;
						writer.Write((byte)1);
						writer.Write(compressedStream.GetBuffer());
					}
				}
			}

			return 0;
		}
	}
}
