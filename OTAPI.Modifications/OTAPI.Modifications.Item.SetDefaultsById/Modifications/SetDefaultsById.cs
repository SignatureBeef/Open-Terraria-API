using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
    public class SetDefaultsById : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.3.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.3.1, Culture=neutral, PublicKeyToken=null"
		};
        public override string Description => "Hooking Item.SetDefaults(int,bool)...";
        public override void Run()
        {
            var vanilla = SourceDefinition.Type("Terraria.Item").Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Boolean
            );


            var cbkBegin = this.ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Item").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Item").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

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
