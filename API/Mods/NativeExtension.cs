using System;

namespace OTA.Mod
{
    public interface INativeMod
    {
        
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NativeModAttribute : Attribute
    {
        public string EntityName { get; set; }

        public NativeModAttribute()
        {
        }

        public NativeModAttribute(string name)
        {
            this.EntityName = name;
        }
    }
}