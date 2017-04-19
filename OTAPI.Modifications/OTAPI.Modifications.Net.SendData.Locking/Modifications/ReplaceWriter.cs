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
			"TerrariaServer, Version=1.3.5.0, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Replacing writer in NetMessage.SendData...";

		public override void Run()
		{
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
			var buffer = sendData.Body.Instructions.First(
				x => x.OpCode == OpCodes.Ldsfld
				&& (x.Operand as FieldReference).Name == "buffer"
			);

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
			if (buffer.Next.OpCode != OpCodes.Ldloc_1)
			{
				throw new NotSupportedException("Expected Ldloc_1");
			}

			/*var*/
			binaryWriter = buffer.Next.OpCode;
			processor.InsertAfter(buffer,
				new { OpCodes.Stloc_1 }
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
					//Needs a local buffer that gets written into our writer

					//find the first argument (ldarg.s number)
					var firstInstruction = writeBuffer.Previous(
						x => x.OpCode == OpCodes.Ldarg_S
						&& (x.Operand as ParameterReference).Name == "number"
					);

					VariableDefinition localBuffer;
					sendData.Body.Variables.Add(localBuffer = new VariableDefinition(
						this.SourceDefinition.MainModule.Import(typeof(byte[]))
					));

					processor.InsertAfter(firstInstruction,
					   //new { OpCodes.Ldc_I4, Operand = 65536 },
					   new { OpCodes.Newarr, Operand = this.SourceDefinition.MainModule.TypeSystem.Byte },
					   new { OpCodes.Stloc, Operand = localBuffer },
					   new { firstInstruction.OpCode, Operand = (ParameterDefinition)firstInstruction.Operand }
				   );

					firstInstruction.OpCode = OpCodes.Ldc_I4;
					firstInstruction.Operand = 65535;

					//find the position set, as we are starting from 0 with out new array
					var argPosition = firstInstruction.Next(x => x.OpCode == OpCodes.Ldloc_1);
					while (argPosition.Next.OpCode != OpCodes.Call)
					{
						processor.Remove(argPosition.Next);
					}
					argPosition.OpCode = OpCodes.Ldc_I4_0;

					vrbBuffer = localBuffer;

					// the local buffer is now in place
					// we now need to send it off to the writer, instead of simply incrementing

					// get the method call and skip the result variable and remove all instructions until the branch out
					var call = writeBuffer.Next(
						x => x.OpCode == OpCodes.Call
						&& (x.Operand as MethodReference).Name == "CompressTileBlock"
					).Next;

					while (call.Next.OpCode != OpCodes.Br)
					{
						processor.Remove(call.Next);
					}

					processor.InsertAfter(call,
						new { OpCode = binaryWriter },
						new { OpCodes.Ldloc, localBuffer },
						new { OpCodes.Ldc_I4_0 },
						new { OpCodes.Ldloc_S, Operand = (VariableDefinition)call.Operand },
						new
						{
							OpCodes.Callvirt,
							Operand = this.SourceDefinition.MainModule.Import(typeof(BinaryWriter)
								.GetMethods()
								.Single(x => x.Name == "Write"
									&& x.GetParameters().Count() == 3
									&& x.GetParameters()[0].ParameterType == typeof(byte[])
								)
							)
						}
					);
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

			if (ScanForWriter())
			{
				throw new NotImplementedException("writeBuffer is still in use!");
			}

			RemoveWriteBuffer();
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