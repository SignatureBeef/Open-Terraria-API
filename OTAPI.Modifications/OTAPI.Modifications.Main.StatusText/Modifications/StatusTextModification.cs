using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Modifications.StatusText.Modifications
{
    /// <summary>
    /// The purpose of this modification is to capture writes to Main.statusText and to mark
    /// it as internal so no one else can use it as it's a terrible field to use in the first place.
    /// </summary>
    public class StatusTextModification : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.1.1, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Patching Main.statusText updates";

        public override void Run()
        {
            //Get the field reference to Terraria.Main.statusText
            var fldStatusText = this.SourceDefinition.Type("Terraria.Main").Field("statusText");

            //Get the method reference to the SetStatusText callback
            var mthSetStatusText = this.SourceDefinition.MainModule.Import(
                this.Method(() => OTAPI.Core.Callbacks.Terraria.Main.SetStatusText(null))
            );

            //Trigger the internal keyword
            fldStatusText.IsFamily = true;
            fldStatusText.IsPublic = false;

            //For each instruction in the assembly we will compare if the Terraria.Main.statusText
            //field is being set with a new value. 
            //If it is, we will replace it so our callback receives the new value instead
            this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
            {
                if (instruction.OpCode == OpCodes.Stsfld && instruction.Operand == fldStatusText)
                {
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = mthSetStatusText;
                }
            });
        }
    }
}
