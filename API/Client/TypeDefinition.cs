#if CLIENT
using System;

namespace OTA.Client
{
    public class TypeDefinition
    {
        public Type InstanceType { get; set; }

        public int TypeId { get; set; }
    }
}
#endif