using System;

namespace OTA
{
    #if Full_API
    /// <summary>
    /// World sender is for world entities, such as NPC or projectiles
    /// </summary>
    public abstract class WorldSender : Terraria.Entity
    #else
    public abstract class WorldSender
    #endif
    {

    }
}