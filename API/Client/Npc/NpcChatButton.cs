using System;

namespace OTA.Client.Npc
{
    /// <summary>
    /// Npc chat buttons in order of display.
    /// </summary>
    /// <example>For the Guide:
    ///     Help = First
    ///     Close = Second
    ///     Crafting = Third</example>
    public enum NpcChatButton : byte
    {
        None = 0,
        First,
        Second,
        Third
    }
}

