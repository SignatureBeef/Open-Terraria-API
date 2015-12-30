using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Client, "Wrapping MapHelper.Initialize")]
        private void HookMapHelperInitialize()
        {
            var target = Terraria.MapHelper.Method("Initialize");

            var apiMatch = API.MapCallback.Methods.Where(x => x.Name.StartsWith("OnMapHelperInitialize"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching Initialize Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            target.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}

