using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modification;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Collision
{
    /// <summary>
    /// This patch adds an extra parameter to SwitchTiles to allow the entity to be passed by callers.
    /// </summary>
    [Ordered(4)] //Change the default order as we need to be infront of the HitSwitch mod.
    public class SwitchTilesEntitiy : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Collision.SwitchTiles\\IEntity...";
        public override void Run(OptionSet options)
        {
            var switchTiles = this.Context.Terraria.Types.Collision.Method("SwitchTiles");
            
            //Add the sender parameter to the vanilla method
            ParameterDefinition prmSender;
            switchTiles.Parameters.Add(prmSender = new ParameterDefinition("sender", ParameterAttributes.None, this.Context.Terraria.Types.Entity)
            {
                HasDefault = true,
                IsOptional = true
            });

            //Update all references to the method so the caller adds themselves (currently they are all senders, woo!)
            this.Context.Terraria.MainModue.ForEachInstruction((mth, ins) =>
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
