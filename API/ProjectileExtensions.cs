#if Full_API
using Terraria;

namespace OTA
{
    /// <summary>
    /// Projectile extensions.
    /// </summary>
    public static class ProjectileExtensions
    {
        /// <summary>
        /// Determines if a projectile is an explosive that causes mass damage.
        /// </summary>
        /// <returns><c>true</c> if is high explosive the specified prj; otherwise, <c>false</c>.</returns>
        /// <param name="prj">Prj.</param>
        public static bool IsHighExplosive(this Projectile prj)
        {
            return prj.type == 29 ||
                    prj.type == 102 ||
                    prj.type == 37 ||
                    prj.type == 108;
        }

        /// <summary>
        /// Determines if the perojectile is a small explosive
        /// </summary>
        /// <returns><c>true</c> if is explosive the specified prj; otherwise, <c>false</c>.</returns>
        /// <param name="prj">Prj.</param>
        public static bool IsExplosive(this Projectile prj)
        {
            return IsHighExplosive(prj) ||
                    prj.type == 30 ||
                    prj.type == 41;
        }
    }
}
#endif