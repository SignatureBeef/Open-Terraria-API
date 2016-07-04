using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Modifications.Hooks.Collision
{
	/// <summary>
	/// This patch adds an extra parameter to SwitchTiles to allow the entity to be passed by callers.
	/// </summary>
	[Ordered(4)] //Change the default order as we need to be infront of the HitSwitch mod.
	public class SwitchTilesEntitiy : ModificationBase
	{
		public override string Description => @"Hooking Collision.SwitchTiles\IEntity...";

		public override void Run(OptionSet options)
		{
			//Import TerrariaEntity as the base type of Terraria.Entity
			this.SourceDefinition.Type("Terraria.Entity").BaseType = this.ModificationDefinition.Type("OTAPI.Core.TerrariaEntity");

			var switchTiles = this.SourceDefinition.Type("Terraria.Collision").Method("SwitchTiles");
			var iEntity = switchTiles.Module.Import(this.ModificationDefinition.Type("OTAPI.Core.IEntity"));

			//Add the sender parameter to the vanilla method
			ParameterDefinition prmSender;
			switchTiles.Parameters.Add(prmSender = new ParameterDefinition("sender", ParameterAttributes.None, iEntity)
			{
				HasDefault = true,
				IsOptional = true
			});

			//Update all references to the method so the caller adds themselves (currently they are all senders, woo!)
			this.SourceDefinition.MainModule.ForEachInstruction((mth, ins) =>
			{
				if (ins.OpCode == OpCodes.Call && ins.Operand == switchTiles)
				{
					var cil = mth.Body.GetILProcessor();
					cil.InsertBefore(ins, cil.Create(OpCodes.Ldarg_0));
				}
			});
		}
	}
}
