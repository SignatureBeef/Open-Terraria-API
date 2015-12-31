#if CLIENT
using System;

namespace OTA.Client.Tile
{
    public class TileDefinition : TypeDefinition
    {
        public OTATile Tile { get; set; }
    }
}
#endif