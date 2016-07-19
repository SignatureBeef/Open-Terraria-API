namespace OTAPI.Core
{
    /// <summary>
    /// IEntity is designed to be the base of all entities in the application.
    /// Said examples of these entities may be:
    ///     Command line sender
    ///     TerrariaEntity (which is Projectiles, Players, Npcs etc) 
    ///     External assembly's implementation
    /// defined in an external assembly.
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// TerrariaEntity is intended to be the inclusive base of all vanilla terraria entities, so will be used instead of the
    /// patcher injecting IEntity.
    /// </summary>
    /// <remarks>
    /// Developers: We must ensure we don't introduce new requirements that conflict with:
    ///     Terraria.Player
    ///     Terraria.NPC
    ///     Terraria.Projectile
    /// </remarks>
    public abstract class TerrariaEntity : IEntity
    {

    }
}