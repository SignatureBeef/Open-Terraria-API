using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [ServerHook]
        private void HookItemSetDefaults()
        {
            var setDefaults = Terraria.Item.Methods.Where(x => x.Name.StartsWith("SetDefault")).ToArray();
            foreach (var method in setDefaults)
            {

                //                method.Body.MaxStackSize = 6;

                var apiMatch = API.ItemCallback.MatchInstanceMethodByParameters("Terraria.Item", method.Parameters, "OnSetDefault");
                if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching SetDefault Begin/End calls in the API");

                var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
                var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

                method.Wrap(cbkBegin, cbkEnd);
                //                method.Body.OptimizeMacros();
                //                method.Body.ComputeOffsets();
            }
        }

        [ServerHook]
        private void HookItemNetDefaults()
        {
            var setDefaults = Terraria.Item.Methods.Single(x => x.Name == "netDefaults");

            var apiMatch = API.ItemCallback.MatchInstanceMethodByParameters("Terraria.Item", setDefaults.Parameters, "OnNetDefaults");
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching netDefaults Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            setDefaults.Wrap(cbkBegin, cbkEnd);
        }
    }
}

