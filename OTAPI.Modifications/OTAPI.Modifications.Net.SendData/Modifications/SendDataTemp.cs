using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
    public class SendDataTemp : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.4.3.0, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Injecting a temporary 11 param NetMessage.SendData...";

        public override void Run()
        {
            var vanilla = this.SourceDefinition.Type("Terraria.NetMessage").Methods.Single(m => m.Name == "SendData");
            vanilla.IsAssembly = true;

            // Define the new SendData method.
            var sendDataTemp = new MethodDefinition("SendData", MethodAttributes.Public | MethodAttributes.Static, this.SourceDefinition.MainModule.TypeSystem.Void);

            var il = sendDataTemp.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Nop));

            // Iterate through the parameters of the original (new) SendData, and we add/clone them to our new method.
            foreach (var var in vanilla.Parameters)
            {
                if (var.Name == "number8")
                    continue;
                sendDataTemp.Parameters.Add(var);
                //Load the parameter value onto the stack.
                il.Emit(OpCodes.Ldarg, var);
            }
            // Load a 0f onto the stack as the last variable.
            il.Emit(OpCodes.Ldc_R4, 0f);

            // Create the method call of the OG SendData.
            var sendDataMethodCall = il.Create(OpCodes.Call, vanilla);
            il.Append(sendDataMethodCall);

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));

            // Add our method to the NetMessage class.
            vanilla.DeclaringType.Methods.Add(sendDataTemp);
        }
    }
}
