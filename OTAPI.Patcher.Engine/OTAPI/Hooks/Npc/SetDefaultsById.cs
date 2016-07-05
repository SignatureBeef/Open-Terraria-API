using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class SetDefaultsById : ModificationBase
	{
		public override string Description => "Hooking Npc.SetDefaults(int,bool)...";
<<<<<<< HEAD
        public override void Run()
        {
            var vanilla = this.Context.Terraria.Types.Npc.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Single
            );
=======
		public override void Run(OptionSet options)
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Methods.Single(
				x => x.Name == "SetDefaults"
				&& x.Parameters.First().ParameterType == SourceDefinition.MainModule.TypeSystem.Int32
				&& x.Parameters.Skip(1).First().ParameterType == SourceDefinition.MainModule.TypeSystem.Single
			);
>>>>>>> 6bfb18a61e1a71ad6b81579632159ccb33bf0800


			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

			vanilla.Wrap(cbkBegin, cbkEnd, true);
		}
	}
}
