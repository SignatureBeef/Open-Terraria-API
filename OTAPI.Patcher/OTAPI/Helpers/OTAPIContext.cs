using OTAPI.Patcher.Modification;

namespace OTAPI.Patcher.Modifications.Helpers
{
    /// <summary>
    /// This defines the OTAPI modification context. It expects that there is an OTAPI.dll loaded (from which has been ILRepacked).
    /// </summary>
    public class OTAPIContext : ModificationContext
    {
        public OTAPIContext()
        {
            OTAPI = new OTAPIOrganiser(this);
            Terraria = new TerrariaOrganiser(this);
        }

        /// <summary>
        /// OTAPI helpers
        /// </summary>
        public OTAPIOrganiser OTAPI { get; }

        /// <summary>
        /// Terraria helpers
        /// </summary>
        public TerrariaOrganiser Terraria { get; }
    }
}
