using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Inject;

namespace OTAPI.Patcher.Modifications.Helpers
{
    public static class OTAPIExtensions
    {
        /// <summary>
        /// Determines the type of the currently-loaded Terraria executable.
        /// </summary>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static TerrariaKind GetTerrariaKind(this Injection<OTAPIContext> injection) =>
            injection.Context.Terraria.Types.Program.Field("IsServer")
                .Constant.Equals(true) ? TerrariaKind.Server : TerrariaKind.Client;

        /// <summary>
        /// Determines if the current terraria assembly is the client binary
        /// </summary>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static bool IsClient(this Injection<OTAPIContext> injection) => injection.GetTerrariaKind() == TerrariaKind.Client;

        /// <summary>
        /// Determines if the current terraria assembly is the server binary
        /// </summary>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static bool IsServer(this Injection<OTAPIContext> injection) => injection.GetTerrariaKind() == TerrariaKind.Server;
    }
}
