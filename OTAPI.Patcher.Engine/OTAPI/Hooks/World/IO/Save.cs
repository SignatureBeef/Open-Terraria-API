using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
	public class Save : ModificationBase
	{
		public override string Description => "Hooking WorldFile.saveWorld(bool,bool)...";
        public override void Run()
        {
			var vanilla = SourceDefinition.Type("Terraria.IO.WorldFile").Methods.Single(
				x => x.Name == "saveWorld"
				&& x.Parameters.Count() == 2
				&& x.Parameters[0].ParameterType == SourceDefinition.MainModule.TypeSystem.Boolean
				&& x.Parameters[1].ParameterType == SourceDefinition.MainModule.TypeSystem.Boolean
			);


			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.WorldFile").Method("SaveWorldBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.WorldFile").Method("SaveWorldEnd", parameters: vanilla.Parameters);

			vanilla.Wrap(cbkBegin, cbkEnd, true);
		}
	}
}
