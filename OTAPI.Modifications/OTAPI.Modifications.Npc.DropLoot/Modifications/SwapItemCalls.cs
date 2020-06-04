using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
    public class DropLoot_1_SwapItemCalls : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.4.0.5, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking Npc.NPCLoot\\NewItem...";
        public override void Run()
        {
            var npcType = this.Type<Terraria.NPC>();
            foreach (var npcLoot in new[] {
                this.Method(() => (new Terraria.NPC()).NPCLoot()),
                this.Method(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC(default, default, default, default)),
            })
            {
                //var dropLoot = SourceDefinition.Type("Terraria.NPC").Method("DropLoot");

                //In this patch we swap all Item.NewItem calls in NPCLoot to use our previously
                //created DropLoot method

                //Gather all Item.NewItem calls
                var itemCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                    && x.Operand is MethodReference
                                    && (x.Operand as MethodReference).Name == "NewItem"
                                    && (x.Operand as MethodReference).DeclaringType.Name == "Item").ToArray();

                var processor = npcLoot.Body.GetILProcessor();

                //Swap each Item.NewItem calls to our Npc.DropLoot method.
                foreach (var call in itemCalls)
                {
                    var parameters = new Mono.Collections.Generic.Collection<ParameterDefinition>(
                        (call.Operand as MethodDefinition).Parameters.ToArray()
                    );
                    parameters.Add(new ParameterDefinition("npc", ParameterAttributes.In, npcType));
                    var dropLoot = npcType.Method("DropLoot",
                        parameters: parameters,
                        skipMethodParameters: 0
                    );

                    //Swap to our custom method
                    call.Operand = dropLoot;

                    //Append the additional arguments to the end of the existing call
                    processor.InsertBefore(call, new[]
                    {
                        new { OpCodes.Ldarg_0 } //Add the npc instance - 'this' for non static, and arg0 for static (Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC)
				    });
                }

                //Ensure the short branches are updated
                npcLoot.Body.SimplifyMacros();
                npcLoot.Body.OptimizeMacros();
            }
        }
    }
}
