
using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Client, "Allowing custom Tiles's to be saved")]
        private void AllowCustomTileSaving()
        {
            var method = Terraria.WorldFile.Method("SaveFileFormatHeader");
            var replacement = API.TileModRegister.Field("MaxId");

            var ldLoc_419 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldc_I4 && x.Operand.Equals(419));

            ldLoc_419.OpCode = OpCodes.Ldsfld;
            ldLoc_419.Operand = Terraria.Import(replacement);
        }
    }
}

