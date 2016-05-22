using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Server, "Forcing Terraria on a diet...", 200)]
        public void SwapTileToMemTile()
        {
            //TODO: check if Terraria.Tile matches the MemTile signatures
            //      it will help to ensure updates are done correctly.

            _asm.MainModule.TryReplaceArrayWithClassInstance(Terraria.Tile, API.MemTile, API.TileCollection, "Tile");

            //Swap the constructor
            var mainCctor = Terraria.Main.Methods.Single(x => x.Name == ".cctor");
            var constructor = _asm.MainModule.Import(_self.MainModule.Types.Single(x => x.Name == "TileCollection").Methods.Single(y => y.Name == ".ctor"));

            //            var il = mainCctor.Body.GetILProcessor();
            var ins = mainCctor.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
                          && x.Operand is MethodReference
                          && (x.Operand as MethodReference).Name == ".ctor"
                          && (x.Operand as MethodReference).DeclaringType is ArrayType
                          && ((x.Operand as MethodReference).DeclaringType as ArrayType).ElementType.Name == "MemTile");
            ins.Operand = constructor;

            ReplaceTilesWithCollection();
        }

        private void ReplaceTilesWithCollection()
        {
            var ts = _self.MainModule.Types.Single(x => x.Name == "TileCollection");
            var tm = _asm.MainModule.Types.Single(x => x.Name == "Main").Fields.Single(x => x.Name == "tile");

            tm.FieldType = _asm.MainModule.Import(ts);
        }
    }
}
