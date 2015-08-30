using System;

namespace OTA.Plugin
{
    /// <summary>
    /// Mark a method as an OTA hook
    /// </summary>
    public class HookAttribute : Attribute
    {
        internal readonly HookOrder order;

        public HookAttribute(HookOrder order = HookOrder.NORMAL)
        {
            this.order = order;
        }
    }
}