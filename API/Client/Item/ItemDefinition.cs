#if CLIENT
using System;

namespace OTA.Client.Item
{
    public class ItemDefinition
    {
        public Type InstanceType { get; set; }

        public int TypeId { get; set; }
    }
}
#endif