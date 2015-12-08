#if CLIENT
using System;

namespace OTA.Client
{
    public interface INativeMod
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NativeModAttribute : Attribute
    {
        public string EntityName { get; set; }

        public NativeModAttribute(string name)
        {
            this.EntityName = name;
        }
    }
}
#endif