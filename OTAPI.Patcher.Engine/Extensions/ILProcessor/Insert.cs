using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions.ILProcessor
{
	public static class InsertExtensions
	{
		/// <summary>
		/// Inserts a group of instructions after the target instruction
		/// </summary>
		public static void InsertAfter(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
		{
			foreach (var instruction in instructions.Reverse())
			{
				processor.InsertAfter(target, instruction);
			}
		}

		/// <summary>
		/// Converts a anonymous type into an Instruction
		/// </summary>
		/// <param name="anon"></param>
		/// <returns></returns>
		public static Instruction AnonymousToInstruction(object anon)
		{
			var annonType = anon.GetType();
			var properties = annonType.GetProperties();

			//An instruction consists of only 1 opcode, or 1 opcode and 1 operation
			if (properties.Length == 0 || properties.Length > 2)
				throw new NotSupportedException("Anonymous instruction expected 1 or 2 properties");

			//Determine the property that contains the OpCode property
			var propOpcode = properties.SingleOrDefault(x => x.PropertyType == typeof(OpCode));
			if (propOpcode == null)
				throw new NotSupportedException("Anonymous instruction expected 1 opcode property");

			//Get the opcode value
			var opcode = (OpCode)propOpcode.GetMethod.Invoke(anon, null);

			//Now determine if we need an operand or not
			Instruction ins = null;
			if (properties.Length == 2)
			{
				//We know we already have the opcode determined, so the second property
				//must be the operand.
				var propOperand = properties.Where(x => x != propOpcode).Single();

				//Now find the Instruction.Create method that takes the same type that is 
				//specified by the operands type.
				//E.g. Instruction.Create(OpCode, FieldReference)
				var instructionMethod = typeof(Instruction).GetMethods()
					.Where(x => x.Name == "Create")
					.Select(x => new { Method = x, Parameters = x.GetParameters() })
					.Where(x => x.Parameters.Length == 2 && x.Parameters[1].ParameterType == propOperand.PropertyType)
					.SingleOrDefault();

				if (instructionMethod == null)
					throw new NotSupportedException($"Instruction.Create does not support type {propOperand.PropertyType.FullName}");

				//Get the operand value and pass it to the Instruction.Create method to create
				//the instruction.
				var operand = propOperand.GetMethod.Invoke(anon, null);
				ins = (Instruction)instructionMethod.Method.Invoke(anon, new[] { opcode, operand });
			}
			else
			{
				//No operand required
				ins = Instruction.Create(opcode);
			}

			return ins;
		}

		/// <summary>
		/// Inserts a list of anonymous instructions after the target instruction
		/// </summary>
		public static List<Instruction> InsertAfter(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, params object[] instructions)
		{
			var created = new List<Instruction>();
			foreach (var anon in instructions.Reverse())
			{
				var ins = AnonymousToInstruction(anon);
				processor.InsertAfter(target, ins);

				created.Add(ins);
			}

			return created;
		}

		/// <summary>
		/// Inserts a list of anonymous instructions before the target instruction
		/// </summary>
		public static List<Instruction> InsertBefore(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, params object[] instructions)
		{
			var created = new List<Instruction>();
			foreach (var anon in instructions)
			{
				var ins = AnonymousToInstruction(anon);
				processor.InsertBefore(target, ins);

				created.Add(ins);
			}

			return created;
		}
	}
}
