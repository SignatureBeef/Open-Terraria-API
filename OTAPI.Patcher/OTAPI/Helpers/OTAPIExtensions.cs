using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modification;

namespace OTAPI.Patcher.Modifications.Helpers
{
    public static class OTAPIExtensions
    {
        /// <summary>
        /// Determines the type of the currently-loaded Terraria executable.
        /// </summary>
        /// <param name="modification"></param>
        /// <returns></returns>
        public static TerrariaKind GetTerrariaKind(this OTAPIModification<OTAPIContext> modification) =>
            modification.Context.Terraria.Types.Program.Field("IsServer")
                .Constant.Equals(true) ? TerrariaKind.Server : TerrariaKind.Client;

        /// <summary>
        /// Determines if the current terraria assembly is the client binary
        /// </summary>
        /// <param name="modification"></param>
        /// <returns></returns>
        public static bool IsClient(this OTAPIModification<OTAPIContext> modification) => modification.GetTerrariaKind() == TerrariaKind.Client;

        /// <summary>
        /// Determines if the current terraria assembly is the server binary
        /// </summary>
        /// <param name="modification"></param>
        /// <returns></returns>
        public static bool IsServer(this OTAPIModification<OTAPIContext> modification) => modification.GetTerrariaKind() == TerrariaKind.Server;
    }
}
