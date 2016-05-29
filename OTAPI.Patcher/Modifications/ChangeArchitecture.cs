using NDesk.Options;
using OTAPI.Patcher.Inject;
using OTAPI.Patcher.Modifications.Helpers;

namespace OTAPI.Patcher.Modifications
{
    /// <summary>
    /// Changes the architecture of the server from
    /// </summary>
    public class ChanegArchitecture : Injection<OTAPIContext>
    {
        public override bool CanInject(OptionSet options) => this.IsServer();

        public override void Inject(OptionSet options)
        {

        }
    }
}
