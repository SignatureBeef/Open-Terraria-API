using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using OTAPI.Tile;

namespace OTAPI.Modification.Tile.Modifications
{
	[Ordered(7)] //After all time modifications as we want to alter the new Terraria.Tile properties (and not fields)
	public class ITileModification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Swapping all Terraria.Tile references to ITile...";

		public override void Run()
		{
			//Get the type definition of Terraria.Tile
			var terrariaTile = this.SourceDefinition.Type("Terraria.Tile");

			var iTile = this.ModificationDefinition.Type("OTAPI.Tile.ITile");
			var importedITile = this.SourceDefinition.MainModule.Import(iTile);

			//Make Terraria.Tile implement ITile
			terrariaTile.Interfaces.Add(importedITile);

			#region Tile constructor
			//Swap all tile constructors to the OTAPI callback
			var createTileCallback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Core.Callbacks.Terraria.Collection.CreateTile())
			);
			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				if (instruction.OpCode == OpCodes.Newobj)
				{
					var operandMethod = instruction.Operand as MethodReference;
					if (operandMethod.DeclaringType.FullName == "Terraria.Tile")
					{
						instruction.OpCode = OpCodes.Call;
						instruction.Operand = createTileCallback;
					}
				}
			});
			#endregion

			#region Terraria.Tile ITile implementations
			foreach (var method in terrariaTile.Methods.Where(m => !m.IsConstructor && !m.IsStatic))
			{
				method.IsFinal = true;
				method.IsVirtual = true;
				method.IsNewSlot = true;
			}
			#endregion

			#region Tile methods
			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var operandMethod = instruction.Operand as MethodDefinition;
				if (operandMethod != null && method.DeclaringType.FullName != "Terraria.Tile")
				{
					if (operandMethod.DeclaringType.FullName == "Terraria.Tile" && !operandMethod.IsStatic)
					{
						//instruction.Operand = iTile.Method(operandMethod.Name, parameters: operandMethod.Parameters, skipParameters: 0);
						var methods = iTile.Methods.Where(mth => mth.Name == operandMethod.Name && mth.Parameters.Count == operandMethod.Parameters.Count);

						if (methods.Count() == 0)
							throw new Exception($"Method `{operandMethod.Name}` is not found on {iTile.FullName}");
						else if (methods.Count() > 1)
							throw new Exception($"Too many methods named `{operandMethod.Name}` found in {iTile.FullName}");

						instruction.Operand = this.SourceDefinition.MainModule.Import(methods.Single());
					}
				}
			});
			#endregion

			#region Tile locals
			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				if (method.HasBody && method.Body.HasVariables)
				{
					foreach (var local in method.Body.Variables)
					{
						if (local.VariableType.FullName == "Terraria.Tile")
						{
							local.VariableType = importedITile;
						}
					}
				}
			});
			#endregion

			#region Method returns
			this.SourceDefinition.MainModule.ForEachMethod(method =>
			{
				if (method.ReturnType.FullName == "Terraria.Tile")
				{
					method.ReturnType = importedITile;
				}
			});
			#endregion

			#region Method parameters
			this.SourceDefinition.MainModule.ForEachMethod(method =>
			{
				if (method.HasParameters)
				{
					foreach (var parameter in method.Parameters)
					{
						if (parameter.ParameterType.FullName == "Terraria.Tile")
						{
							parameter.ParameterType = importedITile;
						}
					}
				}
			});
			#endregion

			#region Method parameters
			this.SourceDefinition.MainModule.ForEachType(type =>
			{
				if (type.HasFields)
				{
					foreach (var field in type.Fields)
					{
						if (field.FieldType.FullName == "Terraria.Tile")
						{
							field.FieldType = importedITile;
						}
					}
				}
			});
			#endregion
		}
	}
}
