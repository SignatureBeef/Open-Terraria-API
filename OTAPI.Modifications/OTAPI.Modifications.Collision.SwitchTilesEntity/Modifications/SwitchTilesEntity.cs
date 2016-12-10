using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Collision
{
	/// <summary>
	/// This patch adds an extra parameter to SwitchTiles to allow the entity to be passed by callers.
	/// </summary>
	[Ordered(4)] //Change the default order as we need to be infront of the HitSwitch mod.
	public class SwitchTilesEntitiy : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => @"Hooking Collision.SwitchTiles\IEntity...";

		public override void Run()
		{
			//Import TerrariaEntity as the base type of Terraria.Entity
			this.Type<Terraria.Entity>().BaseType = SourceDefinition.MainModule.Import(
				this.Type<OTAPI.TerrariaEntity>()
			);

			var switchTiles = this.SourceDefinition.Type("Terraria.Collision").Method("SwitchTiles");
			var iEntity = switchTiles.Module.Import(
				this.Type<IEntity>()
			);

			//Add the sender parameter to the vanilla method
			ParameterDefinition prmSender;
			switchTiles.Parameters.Add(prmSender = new ParameterDefinition("sender", ParameterAttributes.None, iEntity)
			{
				HasDefault = true,
				IsOptional = true,
				Constant = null
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
