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
    }
}

