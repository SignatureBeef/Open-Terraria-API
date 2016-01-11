using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Client, "Wrapping Chest.SetupShop")]
        private void HookChestSetupShop()
        {
            var target = Terraria.Chest.Method("SetupShop");

            var apiMatch = API.ChestCallback.MatchInstanceMethodByParameters("Terraria.Chest", target.Parameters, "OnSetupShop");
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching SetupShop Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            target.Wrap(cbkBegin, cbkEnd, true);
        }

//        [OTAPatch(SupportType.Client, "ABC")]
//        private void PatchWIndows()
//        {
//            var target = Terraria.Program.Method("<LaunchGame>b__0");
//            target.Body.Instructions.Clear();
//            var il = target.Body.GetILProcessor();
//
//            il.Append(il.Create(OpCodes.Ret));
//        }
    }
}

