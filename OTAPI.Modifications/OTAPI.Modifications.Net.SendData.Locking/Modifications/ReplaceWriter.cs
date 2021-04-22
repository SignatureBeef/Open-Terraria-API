using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	[Ordered(4)]
	public class ReplaceWriter : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.2, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Replacing writer in NetMessage.SendData...";

		public override void Run()
		{
			// return;
			// find the first writer and then remove all instructions up 
			// until (an including) the writer position being reset to 0

			var sendData = this.Method(() => Terraria.NetMessage.SendData(0, 0, 0, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0));

			var processor = sendData.Body.GetILProcessor();

			VariableDefinition mswriter;
			OpCode binaryWriter;

			InjectNewWriter(sendData, processor, out mswriter, out binaryWriter);
			SendWriterPacket(sendData, processor, mswriter, binaryWriter);
			NurfWriteBuffer();
		}

		void InjectNewWriter(MethodDefinition sendData, ILProcessor processor, out VariableDefinition mswriter, out OpCode binaryWriter)
		{
			var buffer = sendData.Body.Instructions.Where(
				x => x.OpCode == OpCodes.Ldsfld
				&& (x.Operand as FieldReference).Name == "buffer"
			).Skip(1).First();

			while (!(buffer.Next.OpCode == OpCodes.Callvirt
				&& (buffer.Next.Operand as MethodReference).Name == "set_Position"))
			{
				processor.Remove(buffer.Next);
			}
			processor.Remove(buffer.Next);
			//processor.Remove(buffer);

			//VariableDefinition mswriter;
			sendData.Body.Variables.Add(mswriter = new VariableDefinition("mswriter",
				this.SourceDefinition.MainModule.Import(typeof(MemoryStream))
			));

			var res = processor.InsertBefore(buffer.Previous.Previous,
				new
				{
					OpCodes.Newobj,
					Operand = this.SourceDefinition.MainModule.Import(typeof(MemoryStream)
						.GetConstructors()
						.Single(x => x.GetParameters().Count() == 0)
					)
				},
				new { OpCodes.Stloc, Operand = mswriter },
				new { OpCodes.Ldloc, Operand = mswriter }
			);

			buffer.Previous.Previous.ReplaceTransfer(res[0], sendData);
			processor.Remove(buffer.Previous);
			processor.Remove(buffer.Previous);

			buffer.OpCode = OpCodes.Newobj;
			buffer.Operand = this.SourceDefinition.MainModule.Import(typeof(BinaryWriter)
				.GetConstructors()
				.Single(x => x.GetParameters().Count() == 1)
			);
			if (buffer.Next.OpCode != OpCodes.Ldloc_3)
			{
				throw new NotSupportedException("Expected Ldloc_3");
			}

			/*var*/
			binaryWriter = buffer.Next.OpCode;
			processor.InsertAfter(buffer,
				new { OpCodes.Stloc_3 }
			);
		}

		void SendWriterPacket(MethodDefinition sendData, ILProcessor processor, VariableDefinition mswriter, OpCode binaryWriter)
		{

			// inject the packet contents array after the method updates the packet id.
			// our signature we look for is the last call to update the Position
			var offset = sendData.Body.Instructions.Last(
				x => x.OpCode == OpCodes.Callvirt
				&& (x.Operand as MethodReference).Name == "set_Position"
			);

			VariableDefinition packetContents;
			sendData.Body.Variables.Add(packetContents = new VariableDefinition("packetContents",
				this.SourceDefinition.MainModule.Import(typeof(byte[]))
			));

			processor.InsertAfter(offset,
				new { OpCodes.Ldloc, mswriter },
				new
				{
					OpCodes.Callvirt,
					Operand = this.SourceDefinition.MainModule.Import(typeof(MemoryStream)
						.GetMethods()
						.Single(x => x.Name == "ToArray" && x.GetParameters().Count() == 0)
					)
				},
				new { OpCodes.Stloc, packetContents }
			);

			// replace all instances of NetMessage.buffer[index].writeBuffer with out new packetContents
			foreach (var writeBuffer in sendData.Body.Instructions.Where(
				x => x.OpCode == OpCodes.Ldfld
				&& (x.Operand as FieldReference).Name == "writeBuffer"
			).ToArray())
			{
				// now remove all calls back to the when the buffer is loaded
				// remove the writeBuffer
				// replace the messagebuffer instruction with our packet contents
				// note: always ensure the writeBuffer is below packetContents

				VariableDefinition vrbBuffer = packetContents;
				if (writeBuffer.Offset < offset.Offset)
				{
					processor.Replace(writeBuffer.Previous.Previous.Previous, Instruction.Create(OpCodes.Nop));
					processor.Replace(writeBuffer.Previous.Previous, Instruction.Create(OpCodes.Nop));
					processor.Replace(writeBuffer.Previous, Instruction.Create(OpCodes.Nop));
					processor.Replace(writeBuffer, Instruction.Create(OpCodes.Ldloc_3));

					var call = sendData.Body.Instructions.
						Single(i => i.OpCode == OpCodes.Call && (i.Operand as MethodReference).Name == "CompressTileBlock");

					call.Operand = this.SourceDefinition.MainModule.Import(this.Method(() => Callbacks.Terraria.NetMessage.CompressTileBlock(0, 0, 0, 0, null, 0)));
					continue;
				}

				var loadedBuffer = writeBuffer.Previous(
					x => x.OpCode == OpCodes.Ldsfld
					&& (x.Operand as FieldReference).Name == "buffer"
				);

				while (loadedBuffer.Next != writeBuffer)
				{
					processor.Remove(loadedBuffer.Next);
				}
				processor.Remove(writeBuffer);

				loadedBuffer.OpCode = OpCodes.Ldloc;
				loadedBuffer.Operand = vrbBuffer;

				//if (writeBuffer.Offset < offset.Offset)
				//{
				//	// the local buffer is now in place
				//	// we now need to send it off to the writer, instead of simply incrementing

				//	// remove all tiles AFTER 
				//}
			}
		}

		void NurfWriteBuffer()
		{
            ClearResetWriter();
			RemoveFromReset();
			RemoveFromConstructor();

            // RemoveOnConnectionAcceptedElse();
            RemoveFromKickClient();

            if (ScanForWriter())
			{
				throw new NotImplementedException("writeBuffer is still in use!");
			}

			RemoveWriteBuffer();
		}

        void RemoveOnConnectionAcceptedElse()
        {
            var netplay = this.Type<Terraria.Netplay>();
            var kickClient = netplay.Method("OnConnectionAccepted");
            var procesor = kickClient.Body.GetILProcessor();

            var offset = kickClient.Body.Instructions.First(x => x.OpCode == OpCodes.Ldsfld
                && (x.Operand as FieldReference).Name == "fullBuffer"
                && x.Previous.OpCode == OpCodes.Br_S
            );

            while (offset.Next.OpCode != OpCodes.Call)
            {
                procesor.Remove(offset.Next);
            }
        }

        void RemoveFromKickClient()
        {
            var netplay = this.Type<Terraria.Netplay>();
            var kickClient = netplay.Method("KickClient");
            var procesor = kickClient.Body.GetILProcessor();

            while (kickClient.Body.Instructions.Count > 1)
            {
                procesor.Remove(kickClient.Body.Instructions[0]);
            }
        }

        void ClearResetWriter()
		{
			var resetWriter = this.Method(() => (new Terraria.MessageBuffer()).ResetWriter());
			var procesor = resetWriter.Body.GetILProcessor();
			while (resetWriter.Body.Instructions.Count > 1)
			{
				procesor.Remove(resetWriter.Body.Instructions[0]);
			}
		}

		void RemoveFromReset()
		{
			var reset = this.Method(() => (new Terraria.MessageBuffer()).Reset());
			var procesor = reset.Body.GetILProcessor();

			var offset = reset.Body.Instructions.First(x => x.OpCode == OpCodes.Ldfld
				&& (x.Operand as FieldReference).Name == "writeBuffer"
				&& x.Previous.OpCode == OpCodes.Ldarg_0
			).Previous;

			while (offset.Next.OpCode != OpCodes.Call)
			{
				procesor.Remove(offset.Next);
			}
			procesor.Remove(offset.Next); //call
			procesor.Remove(offset); //ldarg.0
		}

		void RemoveFromConstructor()
		{
			var reset = this.Type<Terraria.MessageBuffer>().Constructor();
			var procesor = reset.Body.GetILProcessor();

			var offset = reset.Body.Instructions.First(x => x.OpCode == OpCodes.Stfld
				&& (x.Operand as FieldReference).Name == "writeBuffer"
			).Previous(x => x.OpCode == OpCodes.Ldarg_0);

			while (offset.Next.OpCode != OpCodes.Stfld)
			{
				procesor.Remove(offset.Next);
			}
			procesor.Remove(offset.Next); //stfld
			procesor.Remove(offset); //ldarg.0
		}

		bool ScanForWriter()
		{
			bool any = false;
			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var field = instruction.Operand as FieldReference;
				if (field != null && field.Name == "writeBuffer" && field.DeclaringType.Name == "MessageBuffer")
				{
					any = true;
				}
			});
			return any;
		}

		void RemoveWriteBuffer()
		{
			this.Type<Terraria.MessageBuffer>().Fields.Remove(
				//this.Field(() => (new Terraria.MessageBuffer()).writeBuffer)
				this.Type<Terraria.MessageBuffer>().Field("writeBuffer") //dont use the typed reference or IL Repack will fail
			);
		}


	}
}
