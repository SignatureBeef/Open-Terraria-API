using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class TypeDefinitionExtensions
	{
		public static void MakeVirtual(this TypeDefinition type)
		{
			var methods = type.Methods.Where(m => !m.IsConstructor && !m.IsStatic).ToArray();
			foreach (var method in methods)
			{
				method.IsVirtual = true;
				method.IsNewSlot = true;
			}

			type.Module.ForEachInstruction((method, instruction) =>
			{
				if (methods.Any(x => x == instruction.Operand))
				{
					if (instruction.OpCode != OpCodes.Callvirt)
					{
						instruction.OpCode = OpCodes.Callvirt;
					}
				}
			});
		}

		/// <summary>
		/// Ensures all members of the type are publicly accessible
		/// </summary>
		/// <param name="type">The type to be made accessible</param>
		/// <param name="nested">To make all nested classes public as well.</param>
		public static void MakePublic(this TypeDefinition type, bool nested = true)
		{
			var state = type.IsPublic;
			if (type.IsNestedFamily)
			{
				type.IsNestedFamily = false;
				type.IsNestedPublic = true;
				state = false;
			}
			if (type.IsNestedFamilyAndAssembly)
			{
				type.IsNestedFamilyAndAssembly = false;
				type.IsNestedPublic = true;
				state = false;
			}
			if (type.IsNestedFamilyOrAssembly)
			{
				type.IsNestedFamilyOrAssembly = false;
				type.IsNestedPublic = true;
				state = false;
			}
			if (type.IsNestedPrivate)
			{
				type.IsNestedPrivate = false;
				type.IsNestedPublic = true;
				state = false;
			}

			type.IsPublic = state;

			foreach (var itm in type.Methods)
			{
				itm.IsPublic = true;
				if (itm.IsFamily) itm.IsFamily = false;
				if (itm.IsFamilyAndAssembly) itm.IsFamilyAndAssembly = false;
				if (itm.IsFamilyOrAssembly) itm.IsFamilyOrAssembly = false;
				if (itm.IsPrivate) itm.IsPrivate = false;
			}
			foreach (var itm in type.Fields)
			{
				if (itm.IsFamily) itm.IsFamily = false;
				if (itm.IsFamilyAndAssembly) itm.IsFamilyAndAssembly = false;
				if (itm.IsFamilyOrAssembly) itm.IsFamilyOrAssembly = false;
				if (itm.IsPrivate)
				{
					if (type.Events.Where(x => x.Name == itm.Name).Count() == 0)
						itm.IsPrivate = false;
					else
					{
						continue;
					}
				}

				itm.IsPublic = true;
			}
			foreach (var itm in type.Properties)
			{
				if (null != itm.GetMethod)
				{
					itm.GetMethod.IsPublic = true;
					if (itm.GetMethod.IsFamily) itm.GetMethod.IsFamily = false;
					if (itm.GetMethod.IsFamilyAndAssembly) itm.GetMethod.IsFamilyAndAssembly = false;
					if (itm.GetMethod.IsFamilyOrAssembly) itm.GetMethod.IsFamilyOrAssembly = false;
					if (itm.GetMethod.IsPrivate) itm.GetMethod.IsPrivate = false;
				}
				if (null != itm.SetMethod)
				{
					itm.SetMethod.IsPublic = true;
					if (itm.SetMethod.IsFamily) itm.SetMethod.IsFamily = false;
					if (itm.SetMethod.IsFamilyAndAssembly) itm.SetMethod.IsFamilyAndAssembly = false;
					if (itm.SetMethod.IsFamilyOrAssembly) itm.SetMethod.IsFamilyOrAssembly = false;
					if (itm.SetMethod.IsPrivate) itm.SetMethod.IsPrivate = false;
				}
			}

			foreach (var nt in type.NestedTypes)
				nt.MakePublic();
		}
	}
}
