using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modifications.Input.Text.Modifications
{
    public class InputTextModification : ModificationBase
    {
        public override IEnumerable<string> AssemblyTargets => new[]
        {
            "Terraria, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
        };

        public override string Description => "Hooking keyboard";

        public override void Run()
        {
            var method = this.SourceDefinition.Type("Terraria.Main").Method("GetInputText");

            string tmp = null;
            var callback = this.Method(() => OTAPI.Callbacks.Terraria.Main.GetInputText(ref tmp, null));

            method.InjectNonVoidBeginCallback(callback);
        }
    }
}
