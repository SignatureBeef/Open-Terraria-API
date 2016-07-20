using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
    public class SetDefaultsById : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking Npc.SetDefaults(int,float)...";
        public override void Run()
        {
            var vanilla = SourceDefinition.Type("Terraria.NPC").Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Single
            );
			
            var cbkBegin = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

            vanilla.Wrap
            (
                beginCallback: cbkBegin,
                endCallback: cbkEnd,
                beginIsCancellable: true,
                noEndHandling: false,
                allowCallbackInstance: true
            );
        }
    }
}
