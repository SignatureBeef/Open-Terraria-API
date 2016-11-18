using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
    public class Load : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking WorldFile.loadWorld(bool)...";
        public override void Run()
        {
            var vanilla = SourceDefinition.Type("Terraria.IO.WorldFile").Methods.Single(
                x => x.Name == "loadWorld"
                && x.Parameters.Count() == 1
                && x.Parameters[0].ParameterType == SourceDefinition.MainModule.TypeSystem.Boolean
            );

            var cbkBegin = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.WorldFile").Method("LoadWorldBegin", parameters: vanilla.Parameters);
            var cbkEnd = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.WorldFile").Method("LoadWorldEnd", parameters: vanilla.Parameters);

            vanilla.Wrap
            (
                beginCallback: cbkBegin,
                endCallback: cbkEnd,
                beginIsCancellable: true,
                noEndHandling: false,
                allowCallbackInstance: false
            );
        }
    }
}
